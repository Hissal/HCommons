using JetBrains.Annotations;

namespace HCommons.Buffers;

[MustDisposeResource]
public ref struct PooledArrayBuilder<T>(int initialCapacity = 4) : IDisposable {
    [HandlesResourceDisposal]
    PooledArray<T> _array = PooledArray<T>.Rent(initialCapacity);
    public int Count { get; private set; } = 0;

    
    public T this[int index] {
        get {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            return _array[index];
        }
        set {
            if (index < 0 || index >= Count)
                throw new IndexOutOfRangeException();
            _array[index] = value;
        }
    }

    public void Add(T item) {
        if (Count >= _array.Length) {
            var newArray = PooledArray<T>.Rent(_array.Length * 2);
            _array.Span.CopyTo(newArray.Span);
            PooledArray<T>.Return(ref _array);
            _array = newArray;
        }
        _array[Count++] = item;
    }
    
    [System.Diagnostics.Contracts.Pure]
    [MustDisposeResource]
    public PooledArray<T> Build() {
        var resultArray = PooledArray<T>.Rent(Count);
        _array.Span[..Count].CopyTo(resultArray.Span);
        return resultArray;
    }

    public void Dispose() {
        PooledArray<T>.Return(ref _array);
        Count = 0;
    }
}