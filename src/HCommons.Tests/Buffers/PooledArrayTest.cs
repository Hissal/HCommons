using System.Buffers;
using HCommons.Buffers;

namespace HCommons.Tests.Buffers;

[Collection(nameof(NonParallelTests))]
[TestSubject(typeof(PooledArray<>))]
public class PooledArrayTest {
    [Fact]
    public void Empty_ShouldReturnAnEmptyPooledArray() {
        var pooled = PooledArray<int>.Empty;
        pooled.Array.ShouldBe([]);
    }
    
    [Fact]
    public void Disposed_ShouldReturnADisposedPooledArray() {
        var pooled = PooledArray<int>.Disposed;
        pooled.IsDisposed.ShouldBe(true);
    }
    
    [Theory]
    [InlineData("Dispose")]
    [InlineData("Return")]
    public void ReturnAndDispose_ShouldReturnArrayToPool(string method) {
        var array = ArrayPool<int>.Shared.Rent(10);
        var pooled = new PooledArray<int>(array, 10);

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
        using var pooled = PooledArray<int>.Rent(10);
        pooled.IsDisposed.ShouldBeFalse();
    }

    [Theory]
    [InlineData("Dispose")]
    [InlineData("Return")]
    public void IsDisposed_ShouldBeTrue_AfterDisposalAndReturn(string method) {
        var pooled = PooledArray<int>.Rent(10);

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
    public void AccessingArray_ShouldNotThrow_BeforeDisposal() {
        using var pooled = PooledArray<int>.Rent(10);
        Should.NotThrow(() => pooled.Array);
    }

    [Theory]
    [InlineData("Dispose")]
    [InlineData("Return")]
    public void AccessingArray_ShouldReturnEmpty_AfterDisposalAndReturn(string method) {
        var pooled = PooledArray<int>.Rent(10);

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

        pooled.Array.ShouldBe([]);
    }

    [Fact]
    public void Length_ShouldBeSetCorrectly_WhenCreatedThroughConstructor() {
        var array = ArrayPool<int>.Shared.Rent(10);
        using var pooled = new PooledArray<int>(array, 10);
        pooled.Length.ShouldBe(10);
    }

    [Fact]
    public void Length_ShouldBeSetCorrectly_WhenRented() {
        using var pooled = PooledArray<int>.Rent(10);
        pooled.Length.ShouldBe(10);
    }
    
    [Theory]
    [InlineData("Dispose")]
    [InlineData("Return")]
    public void Length_ShouldBeZero_AfterDisposalAndReturn(string method) {
        var pooled = PooledArray<int>.Rent(10);

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

        pooled.Length.ShouldBe(0);
    }

    [Fact]
    public void Span_ShouldReturnCorrectSpan() {
        using var pooled = PooledArray<int>.Rent(10);
        var span = pooled.Span;
        span.Length.ShouldBe(10);
    }

    [Fact]
    public void Memory_ShouldReturnCorrectMemory() {
        using var pooled = PooledArray<int>.Rent(10);
        var memory = pooled.Memory;
        memory.Length.ShouldBe(10);
    }

    [Fact]
    public void Segment_ShouldReturnCorrectSegment() {
        using var pooled = PooledArray<int>.Rent(10);
        var segment = pooled.Segment;
        segment.Count.ShouldBe(10);
    }

    [Fact]
    public void Indexer_ShouldGetAndSetValuesCorrectly() {
        var pooled = PooledArray<int>.Rent(10);
        try {
            for (var i = 0; i < pooled.Length; i++) {
                pooled[i] = i * 2;
            }

            for (var i = 0; i < pooled.Length; i++) {
                pooled[i].ShouldBe(i * 2);
            }
        }
        finally {
            pooled.Dispose();
        }
    }

    [Fact]
    public void Indexer_ShouldThrowIndexOutOfRangeException_ForInvalidIndices() {
        var pooled = PooledArray<int>.Rent(10);
        try {
            Should.Throw<IndexOutOfRangeException>(() => pooled[-1]);
            Should.Throw<IndexOutOfRangeException>(() => pooled[10]);
            Should.Throw<IndexOutOfRangeException>(() => { pooled[-1] = 0; });
            Should.Throw<IndexOutOfRangeException>(() => { pooled[10] = 0; });
        }
        finally {
            pooled.Dispose();
        }
    }

    [Fact]
    public void GetEnumerator_ShouldIterateOverElementsCorrectly() {
        var pooled = PooledArray<int>.Rent(5);
        try {
            for (var i = 0; i < pooled.Length; i++) {
                pooled[i] = i + 1;
            }
            
            var expected = 1;
            foreach (var item in pooled) {
                item.ShouldBe(expected);
                expected++;
            }
        }
        finally {
            pooled.Dispose();
        }
    }

    [Fact]
    public void RentCopy_ShouldCreateCopyOfArray() {
        var original = PooledArray<int>.Rent(10);
        try {
            for (var i = 0; i < original.Length; i++) {
                original[i] = i + 1;
            }
        
            using var copy = original.RentCopy();
            for (var i = 0; i < copy.Length; i++) {
                copy[i].ShouldBe(original[i], "copied values should match original");
            }
            
            copy.Length.ShouldBe(original.Length, "lengths should match");
            copy.Array.ShouldNotBeSameAs(original.Array, "arrays should be different instances");
        }
        finally {
            original.Dispose();
        }
    }
    
    [Fact]
    public void RentSlice_ShouldCreateSlicedCopyOfArray() {
        var original = PooledArray<int>.Rent(10);
        try {
            for (var i = 0; i < original.Length; i++) {
                original[i] = i + 1;
            }
        
            using var slice = original.RentSlice(2, 5);
            for (var i = 0; i < slice.Length; i++) {
                slice[i].ShouldBe(original[i + 2], "sliced values should match original");
            }
            
            slice.Length.ShouldBe(5, "slice length should be correct");
            slice.Array.ShouldNotBeSameAs(original.Array, "arrays should be different instances");
        }
        finally {
            original.Dispose();
        }
    }
    
    [Fact]
    public void RentSlice_WithStartOnly_ShouldCreateSlicedCopyToEnd() {
        var original = PooledArray<int>.Rent(10);
        try {
            for (var i = 0; i < original.Length; i++) {
                original[i] = i + 1;
            }
            
            using var slice = original.RentSlice(4);
            for (var i = 0; i < slice.Length; i++) {
                slice[i].ShouldBe(original[i + 4], "sliced values should match original");
            }
            slice.Length.ShouldBe(6, "slice length should be correct");
            slice.Array.ShouldNotBeSameAs(original.Array, "arrays should be different instances");
        }
        finally {
            original.Dispose();
        }
    }
}