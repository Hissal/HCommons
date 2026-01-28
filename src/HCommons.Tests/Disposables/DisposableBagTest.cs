using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(DisposableBag))]
public class DisposableBagTest {
    DisposableBag bag;
    
    public DisposableBagTest() {
        bag = new DisposableBag(4);
    }
    
    [Fact]
    public void Clear_DisposesAll() {
        // Arrange
        var disposables = Enumerable.Range(0, 4).Select(_ => Substitute.For<IDisposable>()).ToArray();
        foreach (var disposable in disposables) {
            bag.Add(disposable);
        }

        // Act
        bag.Clear();

        // Assert
        foreach (var d in disposables) {
            d.Received(1).Dispose();
        }
    }
    
    [Fact]
    public void Clear_MultipleTimes_DisposesAllOnlyOnce() {
        // Arrange
        var disposables = Enumerable.Range(0, 4).Select(_ => Substitute.For<IDisposable>()).ToArray();
        foreach (var disposable in disposables) {
            bag.Add(disposable);
        }

        // Act
        bag.Clear();
        bag.Clear();
        bag.Clear();

        // Assert
        foreach (var d in disposables) {
            d.Received(1).Dispose();
        }
    }
    
    [Fact]
    public void AllowsAddingAfterClear() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();

        // Act
        bag.Clear();
        bag.Add(d1);
        bag.Clear();

        // Assert
        d1.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_DisposesAll() {
        // Arrange
        var disposables = Enumerable.Range(0, 4).Select(_ => Substitute.For<IDisposable>()).ToArray();
        foreach (var disposable in disposables) {
            bag.Add(disposable);
        }

        // Act
        bag.Dispose();

        // Assert
        foreach (var d in disposables) {
            d.Received(1).Dispose();
        }
    }

    [Fact]
    public void Dispose_MultipleTimes_DisposesOnlyOnce() {
        // Arrange
        var disposables = Enumerable.Range(0, 4).Select(_ => Substitute.For<IDisposable>()).ToArray();
        foreach (var disposable in disposables) {
            bag.Add(disposable);
        }

        // Act
        bag.Dispose();
        bag.Dispose();
        bag.Dispose();

        // Assert
        foreach (var d in disposables) {
            d.Received(1).Dispose();
        }
    }

    [Fact]
    public void Add_AfterDisposeImmediatelyDisposes() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();

        // Act
        bag.Dispose();
        bag.Add(d1);

        // Assert
        d1.Received(1).Dispose();
    }
    
    [Fact]
    public void Add_ManyDisposables_WorksCorrectly() {
        // Arrange
        var disposables = Enumerable.Range(0, 1000).Select(_ => Substitute.For<IDisposable>()).ToArray();

        // Act
        foreach (var disposable in disposables) {
            bag.Add(disposable);
        }
        bag.Dispose();

        // Assert
        foreach (var d in disposables) {
            d.Received(1).Dispose();
        }
    }
    
    [Fact]
    public void Clear_WithKeepAllocatedArray_ClearsArraySlots() {
        // Arrange
        var disposable = Substitute.For<IDisposable>();
        bag.Add(disposable);

        // Act
        bag.Clear(keepAllocatedArray: true);
        
        // Add a new item to verify the array was cleared
        var newDisposable = Substitute.For<IDisposable>();
        bag.Add(newDisposable);
        bag.Clear();

        // Assert
        // Both disposables should have been disposed exactly once
        disposable.Received(1).Dispose();
        newDisposable.Received(1).Dispose();
    }

    [Fact]
    public void Clear_WithSingleException_ThrowsAggregateException() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        var d3 = Substitute.For<IDisposable>();
        var exception = new InvalidOperationException("Dispose failed");
        
        d2.When(x => x.Dispose()).Do(_ => throw exception);
        
        bag.Add(d1);
        bag.Add(d2);
        bag.Add(d3);

        // Act
        var aggregateException = Assert.Throws<AggregateException>(() => bag.Clear());

        // Assert
        aggregateException.InnerExceptions.Count.ShouldBe(1);
        aggregateException.InnerExceptions[0].ShouldBe(exception);
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
        d3.Received(1).Dispose();
    }

    [Fact]
    public void Clear_WithMultipleExceptions_ThrowsAggregateExceptionWithAllExceptions() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        var d3 = Substitute.For<IDisposable>();
        var exception1 = new InvalidOperationException("First dispose failed");
        var exception2 = new ArgumentException("Second dispose failed");
        
        d1.When(x => x.Dispose()).Do(_ => throw exception1);
        d3.When(x => x.Dispose()).Do(_ => throw exception2);
        
        bag.Add(d1);
        bag.Add(d2);
        bag.Add(d3);

        // Act
        var aggregateException = Assert.Throws<AggregateException>(() => bag.Clear());

        // Assert
        aggregateException.InnerExceptions.Count.ShouldBe(2);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
        d3.Received(1).Dispose();
    }

    [Fact]
    public void Clear_WithException_StillClearsCount() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        var exception = new InvalidOperationException("Dispose failed");
        
        d1.When(x => x.Dispose()).Do(_ => throw exception);
        
        bag.Add(d1);
        bag.Add(d2);

        // Act
        Should.Throw<AggregateException>(() => bag.Clear());
        
        // Assert - bag should be cleared, allowing new items to be added
        var d3 = Substitute.For<IDisposable>();
        bag.Add(d3);
        bag.Clear();
        d3.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_WithException_ThrowsAggregateException() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        var exception = new InvalidOperationException("Dispose failed");
        
        d1.When(x => x.Dispose()).Do(_ => throw exception);
        
        bag.Add(d1);
        bag.Add(d2);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => bag.Dispose());

        // Assert
        aggregateException.InnerExceptions.Count.ShouldBe(1);
        aggregateException.InnerExceptions[0].ShouldBe(exception);
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
    }

    [Fact]
    public void Dispose_WithException_MarksAsDisposed() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();
        var exception = new InvalidOperationException("Dispose failed");
        
        d1.When(x => x.Dispose()).Do(_ => throw exception);
        bag.Add(d1);

        // Act
        Should.Throw<AggregateException>(() => bag.Dispose());
        
        // Assert - bag should be marked as disposed
        var d2 = Substitute.For<IDisposable>();
        bag.Add(d2);
        
        // d2 should be disposed immediately since bag is disposed
        d2.Received(1).Dispose();
        
        // subsequent Dispose calls should have no effect
        bag.Dispose();
        d1.Received(1).Dispose(); // Still only called once from first Dispose
    }

    [Fact]
    public void Clear_WithNoExceptions_DoesNotThrow() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        
        bag.Add(d1);
        bag.Add(d2);

        // Act & Assert - should not throw
        bag.Clear();
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
    }
}