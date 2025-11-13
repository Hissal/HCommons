using System.Buffers;
using HCommons.Buffers;

namespace HCommons.Tests.Buffers;

[Collection(nameof(NonParallelTests))]
[TestSubject(typeof(ReadOnlyPooledArray<>))]
public class ReadOnlyPooledArrayTest {
    [Theory]
    [InlineData("Dispose")]
    [InlineData("Return")]
    public void ReturnAndDispose_ShouldReturnArrayToPool(string method) {
        var array = ArrayPool<int>.Shared.Rent(10);
        var pooled = new ReadOnlyPooledArray<int>(array, 10);

        switch (method) {
            case "Dispose":
                pooled.Dispose();
                break;
            case "Return":
                PooledArray<int>.Return(ref pooled);
                break;
            default:
                throw new ArgumentException("Invalid method", nameof(method));
        }

        using var pooled2 = PooledArray<int>.Rent(10);
        pooled2.Array.ShouldBeSameAs(array);
    }

    [Fact]
    public void IsDisposed_ShouldBeFalse_BeforeDisposal() {
        using var pooled = PooledArray<int>.Rent(10).AsReadOnly();
        pooled.IsDisposed.ShouldBeFalse();
    }

    [Theory]
    [InlineData("Dispose")]
    [InlineData("Return")]
    public void IsDisposed_ShouldBeTrue_AfterDisposalAndReturn(string method) {
        var pooled = PooledArray<int>.Rent(10).AsReadOnly();
        
        switch (method) {
            case "Dispose":
                pooled.Dispose();
                break;
            case "Return":
                PooledArray<int>.Return(ref pooled);
                break;
            default:
                throw new ArgumentException("Invalid method", nameof(method));
        }
        
        pooled.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void Length_ShouldReturnCorrectly_WhenCreatedThroughConstructor() {
        var array = ArrayPool<int>.Shared.Rent(15);
        using var pooled = new ReadOnlyPooledArray<int>(array, 15);
        pooled.Length.ShouldBe(15);
    }
    
    [Fact]
    public void Length_ShouldReturnCorrectly_WhenConvertedWithAsReadonly() {
        using var pooled = PooledArray<int>.Rent(20).AsReadOnly();
        pooled.Length.ShouldBe(20);
    }
    
    [Fact]
    public void Span_ShouldReturnCorrectSpan() {
        using var pooled = PooledArray<int>.Rent(10).AsReadOnly();
        var span = pooled.Span;
        span.Length.ShouldBe(10);
    }
    
    [Fact]
    public void Memory_ShouldReturnCorrectMemory() {
        using var pooled = PooledArray<int>.Rent(10).AsReadOnly();
        var memory = pooled.Memory;
        memory.Length.ShouldBe(10);
    }
    
    [Fact]
    public void Indexer_ShouldReturnCorrectValue() {
        var pooled = PooledArray<int>.Rent(10);
        for (var i = 0; i < 10; i++) {
            pooled[i] = i * 2;
        }
        using var readOnlyPooled = pooled.AsReadOnly();
        for (var i = 0; i < 10; i++) {
            readOnlyPooled[i].ShouldBe(i * 2);
        }
    }
    
    [Fact]
    public void Indexer_ShouldThrowIndexOutOfRangeException_WhenIndexIsInvalid() {
        using var pooled = PooledArray<int>.Rent(10).AsReadOnly();
        Should.Throw<IndexOutOfRangeException>(() => pooled[-1]);
        Should.Throw<IndexOutOfRangeException>(() => pooled[10]);
    }
    
    [Fact]
    public void GetEnumerator_ShouldIterateOverElementsCorrectly() {
        var pooled = PooledArray<int>.Rent(5);
        for (var i = 0; i < 5; i++) {
            pooled[i] = i + 1;
        }
        using var readOnlyPooled = pooled.AsReadOnly();
        
        var expected = 1;
        foreach (var item in readOnlyPooled) {
            item.ShouldBe(expected);
            expected++;
        }
    }
    
    [Fact]
    public void RentCopy_ShouldCreateCopyOfArray() {
        var pooled = PooledArray<int>.Rent(5);
        for (var i = 0; i < 5; i++) {
            pooled[i] = i + 1;
        }
        using var original = pooled.AsReadOnly();
        using var copy = original.RentCopy();
        
        for (var i = 0; i < 5; i++) {
            copy[i].ShouldBe(original[i]);
        }
        
        copy.Length.ShouldBe(original.Length);
        copy.Memory.Equals(original.Memory).ShouldBeFalse();
    }

    [Fact]
    public void RentSlice_ShouldCreateSlicedCopyOfArray() {
        var pooled = PooledArray<int>.Rent(10);
        for (var i = 0; i < 10; i++) {
            pooled[i] = i + 1;
        }
        using var original = pooled.AsReadOnly();
        using var slice = original.RentSlice(2, 5);
        
        for (var i = 0; i < 5; i++) {
            slice[i].ShouldBe(original[i + 2]);
        }
        
        slice.Length.ShouldBe(5);
        slice.Memory.Equals(original.Memory).ShouldBeFalse();
    }
    
    [Fact]
    public void RentSlice_WithStartOnly_ShouldCreateSlicedCopyOfArray() {
        var pooled = PooledArray<int>.Rent(10);
        for (var i = 0; i < 10; i++) {
            pooled[i] = i + 1;
        }
        using var original = pooled.AsReadOnly();
        using var slice = original.RentSlice(4);
        
        for (var i = 0; i < 6; i++) {
            slice[i].ShouldBe(original[i + 4]);
        }
        
        slice.Length.ShouldBe(6);
        slice.Memory.Equals(original.Memory).ShouldBeFalse();
    }
}