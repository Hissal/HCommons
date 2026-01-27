using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(CompositeDisposable))]
public class CompositeDisposableExceptionHandlingTest {
    [Fact]
    public void Clear_MultipleThrow_AllStillDisposed_ThrowsAggregateException() {
        // Arrange
        var composite = new CompositeDisposable();
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var disposable3 = Substitute.For<IDisposable>();
        var disposable4 = Substitute.For<IDisposable>();
        
        var exception1 = new InvalidOperationException("Exception 1");
        var exception3 = new InvalidOperationException("Exception 3");
        
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        
        composite.Add(disposable1);
        composite.Add(disposable2);
        composite.Add(disposable3);
        composite.Add(disposable4);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => composite.Clear());

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        disposable3.Received(1).Dispose();
        disposable4.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(2);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception3);
    }

    [Fact]
    public void Dispose_MultipleThrow_AllStillDisposed_ThrowsAggregateException() {
        // Arrange
        var composite = new CompositeDisposable();
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var disposable3 = Substitute.For<IDisposable>();
        
        var exception2 = new InvalidOperationException("Exception 2");
        
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        
        composite.Add(disposable1);
        composite.Add(disposable2);
        composite.Add(disposable3);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => composite.Dispose());

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        disposable3.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(1);
        aggregateException.InnerExceptions[0].ShouldBe(exception2);
    }

    [Fact]
    public void Add_WhenDisposed_DisposesItemImmediately_NullSafe() {
        // Arrange
        var composite = new CompositeDisposable();
        composite.Dispose();

        // Act & Assert - Should not throw for null
        Should.NotThrow(() => composite.Add(null!));
    }
}
