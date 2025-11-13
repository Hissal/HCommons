using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts; // for [Pure]
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace HCommons.Buffers;

[MustDisposeResource]
public struct PooledArray<T>(T[] array, int length) : IDisposable {
    T[]? _array = array;
    public T[] Array {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array ?? throw new ObjectDisposedException(nameof(PooledArray<T>));
    }

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
    public static PooledArray<T> Rent(int length) {
        var array = ArrayPool<T>.Shared.Rent(length);
        return new PooledArray<T>(array, length);
    }

    public static void Return([HandlesResourceDisposal] ref PooledArray<T> pooledArray) => pooledArray.Dispose();
    public static void Return([HandlesResourceDisposal] ref PooledArray<T> pooledArray, bool clearArray) => pooledArray.ReturnToPool(clearArray);
    
    public static void Return([HandlesResourceDisposal] ref ReadOnlyPooledArray<T> pooledArray) => pooledArray.Dispose();
    public static void Return([HandlesResourceDisposal] ref ReadOnlyPooledArray<T> pooledArray, bool clearArray) => pooledArray.ReturnToPool(clearArray);

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
    
    [MustDisposeResource]
    [HandlesResourceDisposal]
    public ReadOnlyPooledArray<T> AsReadOnly() {
        if (IsDisposed)
            throw new ObjectDisposedException(nameof(PooledArray<T>));
        
        var array = _array!;
        _array = null!;
        return new ReadOnlyPooledArray<T>(array, Length);
    }
    
    public void Dispose() => ReturnToPool(RuntimeHelpers.IsReferenceOrContainsReferences<T>());

    void ReturnToPool(bool clearArray) {
        if (IsDisposed)
            return;
        
        ArrayPool<T>.Shared.Return(_array, clearArray);
        _array = null!;
    }
    
    public Enumerator GetEnumerator() => new (this);
    
    public struct Enumerator(PooledArray<T> pooledArray) {
        int _index = -1;

        public T Current => pooledArray[_index];

        public bool MoveNext() {
            _index++;
            return _index < pooledArray.Length;
        }
    }
}

[MustDisposeResource]
public struct ReadOnlyPooledArray<T>(T[] array, int length) : IDisposable {
    T[]? _array = array;
    T[] Array {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array ?? throw new ObjectDisposedException(nameof(ReadOnlyPooledArray<T>));
    }

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
        return newArr.AsReadOnly();
    }
    
    public Enumerator GetEnumerator() => new (this);
    
    public void Dispose() => ReturnToPool(RuntimeHelpers.IsReferenceOrContainsReferences<T>());
    
    internal void ReturnToPool(bool clearArray) {
        if (IsDisposed)
            return;
        
        ArrayPool<T>.Shared.Return(_array, clearArray);
        _array = null!;
    }   
    
    public struct Enumerator(ReadOnlyPooledArray<T> pooledArray) {
        int _index = -1;

        public T Current => pooledArray[_index];

        public bool MoveNext() {
            _index++;
            return _index < pooledArray.Length;
        }
    }
}