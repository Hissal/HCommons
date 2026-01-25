using HCommons.Buffers.Internal;

namespace HCommons.Buffers;

/// <summary>
/// Provides extension methods for converting various collection types to <see cref="PooledArray{T}"/>.
/// </summary>
public static class PooledArrayExtensions {
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by copying the contents of a <see cref="Span{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="source">The span to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// </remarks>
    public static PooledArray<T> ToPooledArray<T>(this Span<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by copying the contents of a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="source">The read-only span to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// </remarks>
    public static PooledArray<T> ToPooledArray<T>(this ReadOnlySpan<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by copying the contents of a <see cref="Memory{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory.</typeparam>
    /// <param name="source">The memory to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// </remarks>
    public static PooledArray<T> ToPooledArray<T>(this Memory<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.Span.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by copying the contents of a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the memory.</typeparam>
    /// <param name="source">The read-only memory to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// </remarks>
    public static PooledArray<T> ToPooledArray<T>(this ReadOnlyMemory<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.Span.CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by copying the contents of an <see cref="ArraySegment{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array segment.</typeparam>
    /// <param name="source">The array segment to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// </remarks>
    public static PooledArray<T> ToPooledArray<T>(this ArraySegment<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Count);
        source.AsSpan().CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by copying the contents of an array.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="source">The array to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// </remarks>
    public static PooledArray<T> ToPooledArray<T>(this T[] source) {
        var pooledArray = PooledArray<T>.Rent(source.Length);
        source.AsSpan().CopyTo(pooledArray.Span);
        return pooledArray;
    }
    
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by copying the contents of an <see cref="ICollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// This method uses <see cref="ICollection{T}.CopyTo"/> for efficient copying.
    /// </remarks>
    public static PooledArray<T> ToPooledArray<T>(this ICollection<T> source) {
        var pooledArray = PooledArray<T>.Rent(source.Count);
        source.CopyTo(pooledArray.Array, 0);
        return pooledArray;
    }
    
    /// <summary>
    /// Creates a new <see cref="PooledArray{T}"/> by enumerating and copying the contents of an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="source">The enumerable to copy from.</param>
    /// <returns>A new pooled array containing a copy of the source data. The caller is responsible for disposing the returned array.</returns>
    /// <remarks>
    /// The returned <see cref="PooledArray{T}"/> must be disposed by the caller to return the underlying array to the pool.
    /// </remarks>
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