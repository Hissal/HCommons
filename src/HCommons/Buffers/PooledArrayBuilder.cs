using System.Buffers;
using JetBrains.Annotations;

namespace HCommons.Buffers;

// TODO: optimize to avoid copying and build in chunks

[MustDisposeResource]
public ref struct PooledArrayBuilder<T> : IDisposable {
    [HandlesResourceDisposal]
    PooledArray<T> _array;
    public int Count { get; private set; } = 0;
    public bool IsDisposed => _array.IsDisposed;
    
    // Required to route through primary constructor when called with no arguments
    public PooledArrayBuilder() : this(4) { }
    public PooledArrayBuilder(int initialCapacity = 4) {
        if (initialCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity), "Initial capacity must be greater than zero.");
        
        var arr = ArrayPool<T>.Shared.Rent(initialCapacity);
        _array = new PooledArray<T>(arr, arr.Length);
    }

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
    
    [System.Diagnostics.Contracts.Pure]
    [MustDisposeResource]
    public PooledArray<T> BuildCopy() {
        return !IsDisposed 
            ? _array.Span[..Count].ToPooledArray() 
            : throw new ObjectDisposedException(nameof(PooledArrayBuilder<T>));
    }

    /// <summary>
    /// Builds and returns a <see cref="PooledArray{T}"/> containing the elements added to this builder.
    /// <para>
    /// <b>Ownership Transfer:</b> This method transfers ownership of the underlying array to the returned <see cref="PooledArray{T}"/>.
    /// After calling <c>Build()</c>, the builder is disposed and must not be used again.
    /// The caller is responsible for disposing the returned array.
    /// </para>
    /// </summary>
    /// <exception cref="ObjectDisposedException">Thrown if the builder has already been disposed.</exception>
    [System.Diagnostics.Contracts.Pure]
    [MustDisposeResource]
    [HandlesResourceDisposal]
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

    public void Dispose() {
        if (IsDisposed)
            return;
        
        Count = 0;
        PooledArray<T>.Return(ref _array);
    }
}