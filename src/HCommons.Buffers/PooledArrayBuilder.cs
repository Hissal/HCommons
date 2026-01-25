using System.Buffers;

namespace HCommons.Buffers;

// TODO: optimize to avoid copying and build in chunks

/// <summary>
/// A builder for constructing <see cref="PooledArray{T}"/> instances incrementally using pooled memory.
/// </summary>
/// <typeparam name="T">The type of elements in the builder.</typeparam>
/// <remarks>
/// <para>
/// This ref struct provides a way to build arrays dynamically while using pooled memory to avoid allocations.
/// The builder automatically grows its capacity as elements are added.
/// </para>
/// <para>
/// <b>Important:</b> This is a ref struct and must be disposed to return the underlying pooled array to the pool.
/// Failure to dispose will result in memory leaks.
/// </para>
/// <para>
/// <b>Usage patterns:</b>
/// <list type="bullet">
/// <item>Use <see cref="Add"/> to incrementally add elements to the builder.</item>
/// <item>Call <see cref="BuildAndDispose"/> to get a <see cref="PooledArray{T}"/> and automatically dispose the builder.</item>
/// <item>Call <see cref="BuildCopy"/> to get a copy while keeping the builder valid for further operations.</item>
/// <item>Always dispose the builder when done, either explicitly or via <see cref="BuildAndDispose"/>.</item>
/// </list>
/// </para>
/// </remarks>
public ref struct PooledArrayBuilder<T> : IDisposable {
    PooledArray<T> _array;
    
    /// <summary>
    /// Gets the number of elements that have been added to the builder.
    /// </summary>
    public int Count { get; private set; } = 0;
    
    /// <summary>
    /// Indicates whether the builder has been disposed.
    /// </summary>
    public bool IsDisposed => _array.IsDisposed;
    
    /// <summary>
    /// Initializes a new instance of <see cref="PooledArrayBuilder{T}"/> with the default initial capacity.
    /// </summary>
    public PooledArrayBuilder() : this(4) { }
    
    /// <summary>
    /// Initializes a new instance of <see cref="PooledArrayBuilder{T}"/> with the specified initial capacity.
    /// </summary>
    /// <param name="initialCapacity">The initial capacity of the builder. Must be greater than zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="initialCapacity"/> is less than or equal to zero.</exception>
    /// <remarks>
    /// The actual capacity may be larger than requested due to pooling behavior. The builder will
    /// automatically grow its capacity as elements are added.
    /// </remarks>
    public PooledArrayBuilder(int initialCapacity = 4) {
        if (initialCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity must be greater than zero.");
        
        var arr = ArrayPool<T>.Shared.Rent(initialCapacity);
        _array = new PooledArray<T>(arr, arr.Length);
    }

    /// <summary>
    /// Gets or sets the element at the specified index.
    /// </summary>
    /// <param name="index">The zero-based index of the element to get or set.</param>
    /// <returns>The element at the specified index.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the builder has been disposed.</exception>
    /// <exception cref="IndexOutOfRangeException">Thrown if <paramref name="index"/> is less than zero or greater than or equal to <see cref="Count"/>.</exception>
    /// <remarks>
    /// The index must be within the range [0, <see cref="Count"/>). To add new elements, use <see cref="Add"/>.
    /// </remarks>
    public T this[int index] {
        get {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(PooledArrayBuilder<T>));
            
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            
            return _array[index];
        }
        set {
            if (IsDisposed)
                throw new ObjectDisposedException(nameof(PooledArrayBuilder<T>));
            
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            
            _array[index] = value;
        }
    }

    /// <summary>
    /// Adds an element to the builder.
    /// </summary>
    /// <param name="item">The element to add.</param>
    /// <exception cref="ObjectDisposedException">Thrown if the builder has been disposed.</exception>
    public void Add(T item) {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(PooledArrayBuilder<T>));
        
        if (Count >= _array.Length) {
            var arr = ArrayPool<T>.Shared.Rent(Count * 2);
            _array.Span.CopyTo(arr.AsSpan());
            PooledArray<T>.Return(ref _array);
            _array = new PooledArray<T>(arr, arr.Length);
        }
        _array[Count++] = item;
    }
    
    /// <summary>
    /// Creates a copy of the current builder's contents as a <see cref="PooledArray{T}"/>.
    /// </summary>
    /// <returns>A new <see cref="PooledArray{T}"/> containing a copy of the elements currently in the builder.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the builder has been disposed.</exception>
    /// <remarks>
    /// <para>
    /// This method creates an independent copy with its own backing array. The builder remains valid
    /// and can continue to be used after calling this method. Both the builder and the returned array
    /// must be disposed independently.
    /// </para>
    /// <para>
    /// Use <see cref="BuildAndDispose"/> if you're done with the builder and want to transfer ownership
    /// without creating a copy.
    /// </para>
    /// </remarks>
    public PooledArray<T> BuildCopy() {
        return !IsDisposed 
            ? _array.Span[..Count].ToPooledArray() 
            : throw new ObjectDisposedException(nameof(PooledArrayBuilder<T>));
    }

    /// <summary>
    /// Builds and returns a <see cref="PooledArray{T}"/> containing the elements added to this builder.
    /// </summary>
    /// <returns>A new <see cref="PooledArray{T}"/> containing the built elements.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the builder has already been disposed.</exception>
    /// <remarks>
    /// <para>
    /// <b>Ownership Transfer:</b> This method transfers ownership of the underlying array to the returned
    /// <see cref="PooledArray{T}"/>. After calling this method, the builder is disposed and must not be used again.
    /// The caller is responsible for disposing the returned array.
    /// </para>
    /// <para>
    /// This method avoids creating a copy of the elements, making it more efficient than <see cref="BuildCopy"/>
    /// when you're done with the builder.
    /// </para>
    /// </remarks>
    public PooledArray<T> BuildAndDispose() {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(PooledArrayBuilder<T>));

        // Return a new PooledArray<T> referencing the same buffer, up to Count
        var pooled = new PooledArray<T>(_array.Array, Count);
        // Invalidate this builder
        Count = 0;
        _array = default;
        return pooled;
    }

    /// <summary>
    /// Disposes the builder, returning the underlying pooled array to the pool.
    /// </summary>
    /// <remarks>
    /// After calling this method, the builder is disposed and cannot be used. If you need the built
    /// result, use <see cref="BuildAndDispose"/> or <see cref="BuildCopy"/> before disposing.
    /// </remarks>
    public void Dispose() {
        if (IsDisposed)
            return;
        
        Count = 0;
        PooledArray<T>.Return(ref _array);
    }
}