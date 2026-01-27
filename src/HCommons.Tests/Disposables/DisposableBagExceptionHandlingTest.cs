using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(DisposableBag))]
public class DisposableBagExceptionHandlingTest {
    [Fact]
    public void Clear_MultipleThrow_AllStillDisposed_ThrowsAggregateException() {
        // Arrange
        var bag = new DisposableBag(5);
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var disposable3 = Substitute.For<IDisposable>();
        var disposable4 = Substitute.For<IDisposable>();
        var disposable5 = Substitute.For<IDisposable>();
        
        var exception2 = new InvalidOperationException("Exception 2");
        var exception4 = new InvalidOperationException("Exception 4");
        
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable4.When(x => x.Dispose()).Do(_ => throw exception4);
        
        bag.Add(disposable1);
        bag.Add(disposable2);
        bag.Add(disposable3);
        bag.Add(disposable4);
        bag.Add(disposable5);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => bag.Clear());

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        disposable3.Received(1).Dispose();
        disposable4.Received(1).Dispose();
        disposable5.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(2);
        aggregateException.InnerExceptions[0].ShouldBe(exception2);
        aggregateException.InnerExceptions[1].ShouldBe(exception4);
    }

    [Fact]
    public void Dispose_MultipleThrow_AllStillDisposed_ThrowsAggregateException() {
        // Arrange
        var bag = new DisposableBag(3);
        var disposable1 = Substitute.For<IDisposable>();
        var disposable2 = Substitute.For<IDisposable>();
        var disposable3 = Substitute.For<IDisposable>();
        
        var exception1 = new InvalidOperationException("Exception 1");
        var exception3 = new InvalidOperationException("Exception 3");
        
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        
        bag.Add(disposable1);
        bag.Add(disposable2);
        bag.Add(disposable3);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => bag.Dispose());

        // Assert
        disposable1.Received(1).Dispose();
        disposable2.Received(1).Dispose();
        disposable3.Received(1).Dispose();
        aggregateException.InnerExceptions.Count.ShouldBe(2);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception3);
    }

    [Fact]
    public void Add_WhenDisposed_DisposesItemImmediately_NullSafe() {
        // Arrange
        var bag = new DisposableBag(1);
        bag.Dispose();

        // Act & Assert - Should not throw for null
        Should.NotThrow(() => bag.Add(null!));
    }
}
