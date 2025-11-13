using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace HCommons.Buffers;

/// <summary>
/// Represents a read-only pooled array that must be disposed to return the underlying array to the pool.
/// </summary>
/// <typeparam name="T">The type of elements in the array.</typeparam>
/// <remarks>
/// <para>
/// This struct wraps an array rented from <see cref="ArrayPool{T}.Shared"/> and must be disposed
/// to return the array to the pool. Failure to dispose will result in memory leaks as the array
/// will not be returned to the pool.
/// </para>
/// <para>
/// <b>Ownership semantics:</b> This is a struct type. Copying it (e.g., passing by value or returning)
/// creates a shallow copy that shares the same underlying array, effectively transferring ownership.
/// Only the final owner should dispose the instance - the original holder must not dispose after
/// transferring ownership. Disposing the same array multiple times will corrupt the pool state.
/// </para>
/// <para>
/// <b>Best practices:</b>
/// <list type="bullet">
/// <item>The owner of a <see cref="ReadOnlyPooledArray{T}"/> is responsible for disposing it (use <c>using</c> statements or <see cref="PooledArray{T}.Return(ref ReadOnlyPooledArray{T})"/>).</item>
/// <item><b>Prefer passing <see cref="Span"/> as method argument</b> instead of passing <see cref="ReadOnlyPooledArray{T}"/> directly. This avoids ownership confusion and allows the receiver to decide how to use the data.</item>
/// <item>Returning a <see cref="ReadOnlyPooledArray{T}"/> from a method is valid and transfers ownership to the caller. The caller becomes responsible for disposal.</item>
/// <item>Use <see cref="RentCopy"/> if you need to share the data while retaining ownership of your instance.</item>
/// <item>Pass by <c>ref</c> when you want to share access without transferring ownership.</item>
/// <item>Never dispose the same instance more than once.</item>
/// </list>
/// </para>
/// </remarks>
[MustDisposeResource]
public struct ReadOnlyPooledArray<T>(T[] array, int length) : IDisposable {
    T[]? _array = array;
    T[] Array {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array ?? throw new ObjectDisposedException(nameof(ReadOnlyPooledArray<T>));
    }

    /// <summary>
    /// Gets the logical length of the pooled array.
    /// </summary>
    /// <remarks>
    /// This is the exact length that was requested when renting, not the actual length of the underlying
    /// array which may be larger due to pooling behavior. All operations (indexing, enumeration, etc.)
    /// respect this logical length.
    /// </remarks>
    public int Length { get; } = length;
    
    /// <summary>
    /// Indicates whether the object has been disposed.
    /// </summary>
    [MemberNotNullWhen(false, nameof(_array))]
    public bool IsDisposed => _array == null;

    /// <summary>
    /// Gets a read-only span representing the array contents.
    /// </summary>
    public ReadOnlySpan<T> Span {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array.AsSpan(0, Length);
    }
    
    /// <summary>
    /// Gets a read-only memory representation of the array contents.
    /// </summary>
    public ReadOnlyMemory<T> Memory {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array.AsMemory(0, Length);
    }

    /// <summary>
    /// Gets the element at the specified index.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    /// <exception cref="IndexOutOfRangeException">Thrown if the index is out of range.</exception>
    public T this[int index] {
        get {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();
            return Array[index];
        }
    }

    /// <summary>
    /// Creates an independent copy of the current read-only pooled array with its own backing storage.
    /// </summary>
    /// <returns>A new <see cref="ReadOnlyPooledArray{T}"/> containing a copy of the elements.</returns>
    /// <remarks>
    /// <para>
    /// Use this method when you want to share the data with another owner while retaining ownership
    /// of your original instance. Both the original and the copy have independent backing arrays and
    /// must be disposed independently.
    /// </para>
    /// <para>
    /// This differs from copying the struct value (passing by value), which transfers ownership by
    /// sharing the same underlying array. Use <see cref="RentCopy"/> when both the original and the
    /// copy need to remain valid and independently disposable.
    /// </para>
    /// </remarks>
    [System.Diagnostics.Contracts.Pure]
    [MustDisposeResource]
    public ReadOnlyPooledArray<T> RentCopy() => RentSlice(0, Length);
    
    /// <summary>
    /// Creates a slice of the current read-only pooled array starting at the specified index.
    /// </summary>
    /// <param name="start">The starting index of the slice.</param>
    /// <returns>A new <see cref="ReadOnlyPooledArray{T}"/> containing the sliced elements.</returns>
    /// <remarks>
    /// <para>
    /// This method creates an independent copy with its own backing array containing only the sliced elements.
    /// Both the original and the returned slice have separate backing arrays and must be disposed independently.
    /// </para>
    /// <para>
    /// The original array retains ownership of its backing storage. Use this method when you want to share
    /// a subset of the data while both instances remain valid and independently disposable.
    /// </para>
    /// </remarks>
    [System.Diagnostics.Contracts.Pure]
    [MustDisposeResource]
    public ReadOnlyPooledArray<T> RentSlice(int start) => RentSlice(start, Length - start);
    
    /// <summary>
    /// Creates a slice of the current read-only pooled array starting at the specified index and with the specified length.
    /// </summary>
    /// <param name="start">The starting index of the slice.</param>
    /// <param name="length">The length of the slice.</param>
    /// <returns>A new <see cref="ReadOnlyPooledArray{T}"/> containing the sliced elements.</returns>
    /// <remarks>
    /// <para>
    /// This method creates an independent copy with its own backing array containing only the sliced elements.
    /// Both the original and the returned slice have separate backing arrays and must be disposed independently.
    /// </para>
    /// <para>
    /// The original array retains ownership of its backing storage. Use this method when you want to share
    /// a subset of the data while both instances remain valid and independently disposable.
    /// </para>
    /// </remarks>
    [System.Diagnostics.Contracts.Pure]
    [MustDisposeResource]
    public ReadOnlyPooledArray<T> RentSlice(int start, int length) {
        var slice = Span.Slice(start, length);
        var newArr = PooledArray<T>.Rent(length);
        slice.CopyTo(newArr.Span);
        return newArr.AsReadOnly();
    }
    
    /// <summary>
    /// Disposes the read-only pooled array, returning it to the pool.
    /// </summary>
    /// <remarks>
    /// Arrays containing reference types or types with references are automatically cleared
    /// before being returned to the pool to avoid memory leaks.
    /// </remarks>
    public void Dispose() => ReturnToPool(RuntimeHelpers.IsReferenceOrContainsReferences<T>());
    
    internal void ReturnToPool(bool clearArray) {
        if (IsDisposed)
            return;
        
        ArrayPool<T>.Shared.Return(_array, clearArray);
        _array = null!;
    }   
    
    /// <summary>
    /// Gets an enumerator for the read-only pooled array.
    /// </summary>
    /// <returns>An enumerator for the read-only pooled array.</returns>
    /// <remarks>
    /// The enumerator will iterate exactly <see cref="Length"/> elements, regardless of the underlying
    /// array's actual length which may be larger due to pooling.
    /// </remarks>
    public Enumerator GetEnumerator() => new (this);
    
    /// <summary>
    /// Represents an enumerator for the read-only pooled array.
    /// </summary>
    public struct Enumerator(ReadOnlyPooledArray<T> pooledArray) {
        int _index = -1;
        
        /// <summary>
        /// Gets the current element in the array.
        /// </summary>
        public T Current => pooledArray[_index];
        
        /// <summary>
        /// Advances the enumerator to the next element.
        /// </summary>
        /// <returns>True if the enumerator was successfully advanced; otherwise, false.</returns>
        public bool MoveNext() {
            _index++;
            return _index < pooledArray.Length;
        }
    }
}