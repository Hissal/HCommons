using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace HCommons.Buffers;

[MustDisposeResource]
public struct PooledArray<T>(T[] array, int length) : IDisposable {
    T[]? _array = array;
    public T[] Array => _array ?? throw new ObjectDisposedException(nameof(PooledArray<T>));
    
    public int Length { get; } = length;

    [MemberNotNullWhen(false, nameof(_array))]
    public bool IsDisposed => _array == null;
    
    public Span<T> Span {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array.AsSpan(0, Length);
    }
    
    public Memory<T> Memory {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array.AsMemory(0, Length);
    }
    
    public ArraySegment<T> Segment {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => new(Array, 0, Length);
    }

    public T this[int index] {
        get {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();
            return Array[index];
        }
        set {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();
            Array[index] = value;
        }
    }
    
    [Pure]
    [MustDisposeResource]
    public static implicit operator ReadOnlyPooledArray<T>(PooledArray<T> pooledArray) => new (pooledArray.Array, pooledArray.Length);

    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> Rent(int length) {
        var array = ArrayPool<T>.Shared.Rent(length);
        return new PooledArray<T>(array, length);
    }

    public static void Return([HandlesResourceDisposal] ref PooledArray<T> pooledArray) => pooledArray.Dispose();
    public static void Return([HandlesResourceDisposal] ref ReadOnlyPooledArray<T> pooledArray) => pooledArray.Dispose();

    [Pure]
    [MustDisposeResource]
    public PooledArray<T> RentCopy() => RentSlice(0, Length);
    [Pure]
    [MustDisposeResource]
    public PooledArray<T> RentSlice(int start) => RentSlice(start, Length - start);
    [Pure]
    [MustDisposeResource]
    public PooledArray<T> RentSlice(int start, int length) {
        var slice = Span.Slice(start, length);
        var newArr = Rent(length);
        slice.CopyTo(newArr.Span);
        return newArr;
    }
    
    public void Dispose() {
        if (IsDisposed)
            return;
        
        ArrayPool<T>.Shared.Return(_array, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        _array = null!;
    }
    
    public PooledArrayEnumerator<T> GetEnumerator() => new (this);
}

[MustDisposeResource]
public struct ReadOnlyPooledArray<T>(T[] array, int length) : IDisposable {
    T[]? _array = array;
    T[] Array => _array ?? throw new ObjectDisposedException(nameof(ReadOnlyPooledArray<T>));
    
    public int Length { get; } = length;
    
    [MemberNotNullWhen(false, nameof(_array))]
    public bool IsDisposed => _array == null;

    public ReadOnlySpan<T> Span {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array.AsSpan(0, Length);
    }
    
    public ReadOnlyMemory<T> Memory {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array.AsMemory(0, Length);
    }

    public T this[int index] {
        get {
            if (index < 0 || index >= Length)
                throw new IndexOutOfRangeException();
            return Array[index];
        }
    }

    [Pure]
    [MustDisposeResource]
    public ReadOnlyPooledArray<T> RentCopy() => RentSlice(0, Length);
    [Pure]
    [MustDisposeResource]
    public ReadOnlyPooledArray<T> RentSlice(int start) => RentSlice(start, Length - start);
    [Pure]
    [MustDisposeResource]
    public ReadOnlyPooledArray<T> RentSlice(int start, int length) {
        var slice = Span.Slice(start, length);
        var newArr = PooledArray<T>.Rent(length);
        slice.CopyTo(newArr.Span);
        return newArr;
    }
    
    public PooledArrayEnumerator<T> GetEnumerator() => new (this);
    
    public void Dispose() {
        if (IsDisposed)
            return;
        
        ArrayPool<T>.Shared.Return(_array, RuntimeHelpers.IsReferenceOrContainsReferences<T>());
        _array = null!;
    }
}

public struct PooledArrayEnumerator<T>(ReadOnlyPooledArray<T> pooledArray) {
    int _index = -1;

    public T Current => pooledArray[_index];

    public bool MoveNext() {
        _index++;
        return _index < pooledArray.Length;
    }
}