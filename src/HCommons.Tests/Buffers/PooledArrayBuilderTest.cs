using HCommons.Buffers;

namespace HCommons.Tests.Buffers;

[Collection(nameof(NonParallelTests))]
[TestSubject(typeof(PooledArrayBuilder<>))]
public class PooledArrayBuilderTest {
    [Fact]
    public void Add_ShouldIncreaseCountAndStoreItem() {
        using var builder = new PooledArrayBuilder<int>();
        builder.Add(42);

        builder.Count.ShouldBe(1);
        builder[0].ShouldBe(42);
    }

    [Fact]
    public void Add_ShouldResizeWhenCapacityExceeded() {
        using var builder = new PooledArrayBuilder<int>(2);
        builder.Add(1);
        builder.Add(2);
        builder.Add(3);

        builder.Count.ShouldBe(3);
        builder[2].ShouldBe(3);
    }

    [Fact]
    public void BuildCopy_ShouldReturnArrayWithCorrectContents() {
        using var builder = new PooledArrayBuilder<int>();
        builder.Add(1);
        builder.Add(2);

        using var result = builder.BuildCopy();

        result.Length.ShouldBe(2);
        result[0].ShouldBe(1);
        result[1].ShouldBe(2);
    }

    [Fact]
    public void Count_ShouldBeZero_AfterDispose() {
        var builder = new PooledArrayBuilder<int>();
        builder.Add(1);
        builder.Dispose();

        builder.Count.ShouldBe(0);
    }

    [Fact]
    public void IsDisposed_ShouldBeFalse_BeforeDispose() {
        using var builder = new PooledArrayBuilder<int>();
        builder.IsDisposed.ShouldBeFalse();
    }
    
    [Fact]
    public void IsDisposed_ShouldBeTrueAfterDispose() {
        var builder = new PooledArrayBuilder<int>();
        builder.Dispose();
        builder.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void IsDisposed_ShouldBeTrue_AfterBuildAndDispose() {
        var builder = new PooledArrayBuilder<int>();
        builder.Add(1);
        using var _ = builder.BuildAndDispose();
    }
    
    [Fact]
    public void Indexer_ShouldThrowIndexOutOfRangeException_ForInvalidIndex() {
        Assert.Throws<IndexOutOfRangeException>(() => {
            using var builder = new PooledArrayBuilder<int>();
            _ = builder[-1];
        });
        
        Assert.Throws<IndexOutOfRangeException>(() => {
            using var builder = new PooledArrayBuilder<int>();
            _ = builder[0];
        });
    }
    
    [Fact]
    public void Indexer_AfterDispose_ShouldThrowObjectDisposedException() {
        Should.Throw<ObjectDisposedException>(() => {
            var builder = new PooledArrayBuilder<int>();
            builder.Dispose();
            _ = builder[0];
        });
        
        Should.Throw<ObjectDisposedException>(() => {
            var builder = new PooledArrayBuilder<int>();
            builder.Dispose();
            builder[0] = 5;
        });
    }
    
    [Fact]
    public void Add_AfterDispose_ShouldThrowObjectDisposedException() {
        Should.Throw<ObjectDisposedException>(() => {
            var builder = new PooledArrayBuilder<int>();
            builder.Dispose();
            builder.Add(2);
        });
    }
    
    [Fact]
    public void BuildCopy_AfterDispose_ShouldThrowObjectDisposedException() {
        Should.Throw<ObjectDisposedException>(() => {
            var builder = new PooledArrayBuilder<int>();
            builder.Dispose();
            using var pooled = builder.BuildCopy();
        });
    }
    
    [Fact]
    public void BuildAndDispose_AfterDispose_ShouldThrowObjectDisposedException() {
        Should.Throw<ObjectDisposedException>(() => {
            var builder = new PooledArrayBuilder<int>();
            builder.Dispose();
            using var pooled = builder.BuildAndDispose();
        });
    }
    
    [Fact]
    public void MultipleDisposeCalls_ShouldNotThrow() {
        var builder = new PooledArrayBuilder<int>();
        builder.Dispose();
        builder.Dispose();
    }
    
    [Fact]
    public void BuildCopy_ShouldReturnEmptyArray_WhenNoItemsAdded() {
        using var builder = new PooledArrayBuilder<int>();
        using var result = builder.BuildCopy();
        
        result.Length.ShouldBe(0);
        result.IsDisposed.ShouldBeFalse();
        result.Array.ShouldBeEmpty();
    }
}