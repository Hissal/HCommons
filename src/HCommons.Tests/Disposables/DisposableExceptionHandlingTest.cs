using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(Disposable))]
public class DisposableExceptionHandlingTest {
    [Fact]
    public void Dispose_TwoDisposables_FirstThrows_SecondStillDisposed_ThrowsAggregateException() {
        // Arrange
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var exception1 = new InvalidOperationException("Test exception 1");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => Disposable.Dispose(disposable1, disposable2));

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(1);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
    }

    [Fact]
    public void Dispose_TwoDisposables_BothThrow_ThrowsAggregateExceptionWithBoth() {
        // Arrange
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var exception1 = new InvalidOperationException("Test exception 1");
        var exception2 = new InvalidOperationException("Test exception 2");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => Disposable.Dispose(disposable1, disposable2));

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(2);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
    }

    [Fact]
    public void Dispose_ThreeDisposables_MiddleThrows_AllStillDisposed() {
        // Arrange
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var disposable3 = Substitute.For<IDisposable>();
        var exception2 = new InvalidOperationException("Test exception 2");
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => 
            Disposable.Dispose(disposable1, disposable2, disposable3));

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        disposable3.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(1);
        aggregateException.InnerExceptions[0].ShouldBe(exception2);
    }

    [Fact]
    public void Dispose_Array_MultipleThrow_AllStillDisposed() {
        // Arrange
        var disposables = new IDisposable[5];
        var exceptions = new Exception[3];
        for (var i = 0; i < 5; i++) {
            disposables[i] = Substitute.For<IDisposable>();
        }
        
        exceptions[0] = new InvalidOperationException("Exception 1");
        exceptions[1] = new InvalidOperationException("Exception 2");
        exceptions[2] = new InvalidOperationException("Exception 3");
        
        disposables[1].When(x => x.Dispose()).Do(_ => throw exceptions[0]);
        disposables[2].When(x => x.Dispose()).Do(_ => throw exceptions[1]);
        disposables[4].When(x => x.Dispose()).Do(_ => throw exceptions[2]);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => Disposable.Dispose(disposables));

        // Assert
        foreach (var disposable in disposables) {
            disposable.Received(1).Dispose();
        }
        aggregateException.InnerExceptions.Count.ShouldBe(3);
        aggregateException.InnerExceptions[0].ShouldBe(exceptions[0]);
        aggregateException.InnerExceptions[1].ShouldBe(exceptions[1]);
        aggregateException.InnerExceptions[2].ShouldBe(exceptions[2]);
    }

    [Fact]
    public void Dispose_NullDisposables_DoesNotThrow() {
        // Act & Assert
        Should.NotThrow(() => Disposable.Dispose(null!, null!));
    }

    [Fact]
    public void CombinedDisposable_MultipleThrow_AllStillDisposed() {
        // Arrange
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var disposable3 = Substitute.For<IDisposable>();
        var exception1 = new InvalidOperationException("Test exception 1");
        var exception3 = new InvalidOperationException("Test exception 3");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        disposable3.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(2);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception3);
    }
}
