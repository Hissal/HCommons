using System.Diagnostics.Contracts; // for [Pure]
using Pure = System.Diagnostics.Contracts.PureAttribute;
using JetBrains.Annotations;


namespace HCommons.Buffers;

public static class PooledArrayExtensions {
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this Span<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this ReadOnlySpan<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this Memory<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.Span.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this ReadOnlyMemory<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.Span.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this ArraySegment<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Count);
        source.AsSpan().CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this T[] source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.AsSpan().CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this ICollection<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Count);
        source.CopyTo(pooledArray.Array, 0);
        return pooledArray;
    }
    
    [Pure]
    [MustDisposeResource]
    public static PooledArray<T> ToPooledArray<T>(this IEnumerable<T> source) {
        if (source.TryGetNonEnumeratedCount(out var count)) {
            var pooledArray = PooledArray<T>.Rent(count);
            try {
                switch (source) {
                    case T[] arr:
                        arr.AsSpan().CopyTo(pooledArray.Span);
                        return pooledArray;
                    case ICollection<T> collection:
                        collection.CopyTo(pooledArray.Array, 0);
                        return pooledArray;
                }

                var i = 0;
                foreach (var item in source) {
                    pooledArray[i++] = item;
                }
                return pooledArray;
            }
            catch {
                PooledArray<T>.Return(ref pooledArray);
                throw;
            }
        }
        
        var builder = new PooledArrayBuilder<T>();
        foreach (var item in source) {
            builder.Add(item);
        }
        return builder.BuildAndDispose();
    }
}