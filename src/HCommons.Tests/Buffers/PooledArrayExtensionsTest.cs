using System.Collections;
using HCommons.Buffers;

namespace HCommons.Tests.Buffers;

[Collection(nameof(NonParallelTests))]
[TestSubject(typeof(PooledArrayExtensions))]
public class PooledArrayExtensionsTest {

    [Fact]
    public void ToPooledArray_Span_Works() {
        int[] array = [1, 2, 3, 4, 5];
        using var pooled = array.AsSpan().ToPooledArray();
        pooled.Span.SequenceEqual(array).ShouldBeTrue();
    }

    [Fact]
    public void ToPooledArray_ReadOnlySpan_Works() {
        int[] array = [1, 2, 3, 4, 5];
        using var pooled = ((ReadOnlySpan<int>)array).ToPooledArray();
        pooled.Span.SequenceEqual(array).ShouldBeTrue();
    }

    [Fact]
    public void ToPooledArray_Memory_Works() {
        int[] array = [1, 2, 3, 4, 5];
        using var pooled = array.AsMemory().ToPooledArray();
        pooled.Span.SequenceEqual(array).ShouldBeTrue();
    }

    [Fact]
    public void ToPooledArray_ReadOnlyMemory_Works() {
        int[] array = [1, 2, 3, 4, 5];
        using var pooled = ((ReadOnlyMemory<int>)array.AsMemory()).ToPooledArray();
        pooled.Span.SequenceEqual(array).ShouldBeTrue();
    }

    [Fact]
    public void ToPooledArray_ArraySegment_Works() {
        int[] array = [1, 2, 3, 4, 5];
        var segment = new ArraySegment<int>(array, 0, array.Length);
        using var pooled = segment.ToPooledArray();
        pooled.Span.SequenceEqual(array).ShouldBeTrue();
    }

    [Fact]
    public void ToPooledArray_Array_Works() {
        int[] array = [1, 2, 3, 4, 5];
        using var pooled = array.ToPooledArray();
        pooled.Span.SequenceEqual(array).ShouldBeTrue();
    }

    [Fact]
    public void ToPooledArray_ICollection_Works() {
        ICollection<int> collection = new List<int> { 1, 2, 3, 4, 5 };
        using var pooled = collection.ToPooledArray();
        pooled.Span.SequenceEqual(collection.ToArray()).ShouldBeTrue();
    }

    [Fact]
    public void ToPooledArray_AnonymousIEnumerable_Works() {
        IEnumerable<int> enumerable = new AnonymousEnumerable<int>([1, 2, 3, 4, 5]);
        using var pooled = enumerable.ToPooledArray();
        pooled.Span.SequenceEqual(enumerable.ToArray()).ShouldBeTrue();
    }
    
    class AnonymousEnumerable<T>(IEnumerable<T> inner) : IEnumerable<T> {
        public IEnumerator<T> GetEnumerator() => inner.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}