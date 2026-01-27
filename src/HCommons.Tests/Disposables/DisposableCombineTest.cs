using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(Disposable))]
public class DisposableCombineTest {
    readonly IDisposable disposable1;
    readonly IDisposable disposable2;
    readonly IDisposable disposable3;
    readonly IDisposable disposable4;
    readonly IDisposable disposable5;
    readonly IDisposable disposable6;
    readonly IDisposable disposable7;
    readonly IDisposable disposable8;
    readonly IDisposable disposable9;
    
    public DisposableCombineTest() {
        disposable1 = Substitute.For<IDisposable>();
        disposable2 = Substitute.For<IDisposable>();
        disposable3 = Substitute.For<IDisposable>();
        disposable4 = Substitute.For<IDisposable>();
        disposable5 = Substitute.For<IDisposable>();
        disposable6 = Substitute.For<IDisposable>();
        disposable7 = Substitute.For<IDisposable>();
        disposable8 = Substitute.For<IDisposable>();
        disposable9 = Substitute.For<IDisposable>();
    }

    [Fact]
    public void Combine_Two_DisposesBoth() {
        var combined = Disposable.Combine(disposable1, disposable2);
        combined.Dispose();
        Assert.All([disposable1, disposable2], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Three_DisposesAll() {
        var combined = Disposable.Combine(disposable1, disposable2, disposable3);
        combined.Dispose();
        Assert.All([disposable1, disposable2, disposable3], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Four_DisposesAll() {
        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4);
        combined.Dispose();
        Assert.All([disposable1, disposable2, disposable3, disposable4], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Five_DisposesAll() {
        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5);
        combined.Dispose();
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Six_DisposesAll() {
        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6);
        combined.Dispose();
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Seven_DisposesAll() {
        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7);
        combined.Dispose();
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Eight_DisposesAll() {
        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7, disposable8);
        combined.Dispose();
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7, disposable8
        ], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Nine_DisposesAll_ViaParams() {
        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7, disposable8, disposable9);
        combined.Dispose();
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7, disposable8, disposable9
        ], d => d.Received(1).Dispose());
    }

    [Fact]
    public void Combine_Two_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);

        var combined = Disposable.Combine(disposable1, disposable2);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(2);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
    }

    [Fact]
    public void Combine_Three_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        var exception3 = new InvalidOperationException("Third failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2, disposable3], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(3);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        aggregateException.InnerExceptions[2].ShouldBe(exception3);
    }

    [Fact]
    public void Combine_Four_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        var exception3 = new InvalidOperationException("Third failed");
        var exception4 = new InvalidOperationException("Fourth failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        disposable4.When(x => x.Dispose()).Do(_ => throw exception4);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2, disposable3, disposable4], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(4);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        aggregateException.InnerExceptions[2].ShouldBe(exception3);
        aggregateException.InnerExceptions[3].ShouldBe(exception4);
    }

    [Fact]
    public void Combine_Five_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        var exception3 = new InvalidOperationException("Third failed");
        var exception4 = new InvalidOperationException("Fourth failed");
        var exception5 = new InvalidOperationException("Fifth failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        disposable4.When(x => x.Dispose()).Do(_ => throw exception4);
        disposable5.When(x => x.Dispose()).Do(_ => throw exception5);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(5);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        aggregateException.InnerExceptions[2].ShouldBe(exception3);
        aggregateException.InnerExceptions[3].ShouldBe(exception4);
        aggregateException.InnerExceptions[4].ShouldBe(exception5);
    }

    [Fact]
    public void Combine_Six_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        var exception3 = new InvalidOperationException("Third failed");
        var exception4 = new InvalidOperationException("Fourth failed");
        var exception5 = new InvalidOperationException("Fifth failed");
        var exception6 = new InvalidOperationException("Sixth failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        disposable4.When(x => x.Dispose()).Do(_ => throw exception4);
        disposable5.When(x => x.Dispose()).Do(_ => throw exception5);
        disposable6.When(x => x.Dispose()).Do(_ => throw exception6);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(6);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        aggregateException.InnerExceptions[2].ShouldBe(exception3);
        aggregateException.InnerExceptions[3].ShouldBe(exception4);
        aggregateException.InnerExceptions[4].ShouldBe(exception5);
        aggregateException.InnerExceptions[5].ShouldBe(exception6);
    }

    [Fact]
    public void Combine_Seven_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        var exception3 = new InvalidOperationException("Third failed");
        var exception4 = new InvalidOperationException("Fourth failed");
        var exception5 = new InvalidOperationException("Fifth failed");
        var exception6 = new InvalidOperationException("Sixth failed");
        var exception7 = new InvalidOperationException("Seventh failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        disposable4.When(x => x.Dispose()).Do(_ => throw exception4);
        disposable5.When(x => x.Dispose()).Do(_ => throw exception5);
        disposable6.When(x => x.Dispose()).Do(_ => throw exception6);
        disposable7.When(x => x.Dispose()).Do(_ => throw exception7);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(7);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        aggregateException.InnerExceptions[2].ShouldBe(exception3);
        aggregateException.InnerExceptions[3].ShouldBe(exception4);
        aggregateException.InnerExceptions[4].ShouldBe(exception5);
        aggregateException.InnerExceptions[5].ShouldBe(exception6);
        aggregateException.InnerExceptions[6].ShouldBe(exception7);
    }

    [Fact]
    public void Combine_Eight_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        var exception3 = new InvalidOperationException("Third failed");
        var exception4 = new InvalidOperationException("Fourth failed");
        var exception5 = new InvalidOperationException("Fifth failed");
        var exception6 = new InvalidOperationException("Sixth failed");
        var exception7 = new InvalidOperationException("Seventh failed");
        var exception8 = new InvalidOperationException("Eighth failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        disposable4.When(x => x.Dispose()).Do(_ => throw exception4);
        disposable5.When(x => x.Dispose()).Do(_ => throw exception5);
        disposable6.When(x => x.Dispose()).Do(_ => throw exception6);
        disposable7.When(x => x.Dispose()).Do(_ => throw exception7);
        disposable8.When(x => x.Dispose()).Do(_ => throw exception8);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, 
            disposable5, disposable6, disposable7, disposable8);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7, disposable8], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(8);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        aggregateException.InnerExceptions[2].ShouldBe(exception3);
        aggregateException.InnerExceptions[3].ShouldBe(exception4);
        aggregateException.InnerExceptions[4].ShouldBe(exception5);
        aggregateException.InnerExceptions[5].ShouldBe(exception6);
        aggregateException.InnerExceptions[6].ShouldBe(exception7);
        aggregateException.InnerExceptions[7].ShouldBe(exception8);
    }

    [Fact]
    public void Combine_ParamsArray_WhenAllThrow_DisposesAllAndAggregatesExceptions() {
        // Arrange
        var exception1 = new InvalidOperationException("First failed");
        var exception2 = new InvalidOperationException("Second failed");
        var exception3 = new InvalidOperationException("Third failed");
        var exception4 = new InvalidOperationException("Fourth failed");
        var exception5 = new InvalidOperationException("Fifth failed");
        var exception6 = new InvalidOperationException("Sixth failed");
        var exception7 = new InvalidOperationException("Seventh failed");
        var exception8 = new InvalidOperationException("Eighth failed");
        var exception9 = new InvalidOperationException("Ninth failed");
        disposable1.When(x => x.Dispose()).Do(_ => throw exception1);
        disposable2.When(x => x.Dispose()).Do(_ => throw exception2);
        disposable3.When(x => x.Dispose()).Do(_ => throw exception3);
        disposable4.When(x => x.Dispose()).Do(_ => throw exception4);
        disposable5.When(x => x.Dispose()).Do(_ => throw exception5);
        disposable6.When(x => x.Dispose()).Do(_ => throw exception6);
        disposable7.When(x => x.Dispose()).Do(_ => throw exception7);
        disposable8.When(x => x.Dispose()).Do(_ => throw exception8);
        disposable9.When(x => x.Dispose()).Do(_ => throw exception9);

        var combined = Disposable.Combine(disposable1, disposable2, disposable3, disposable4, 
            disposable5, disposable6, disposable7, disposable8, disposable9);

        // Act
        var aggregateException = Should.Throw<AggregateException>(() => combined.Dispose());

        // Assert
        Assert.All([disposable1, disposable2, disposable3, disposable4, disposable5, disposable6, disposable7, disposable8, disposable9], d => d.Received(1).Dispose());
        aggregateException.InnerExceptions.Count.ShouldBe(9);
        aggregateException.InnerExceptions[0].ShouldBe(exception1);
        aggregateException.InnerExceptions[1].ShouldBe(exception2);
        aggregateException.InnerExceptions[2].ShouldBe(exception3);
        aggregateException.InnerExceptions[3].ShouldBe(exception4);
        aggregateException.InnerExceptions[4].ShouldBe(exception5);
        aggregateException.InnerExceptions[5].ShouldBe(exception6);
        aggregateException.InnerExceptions[6].ShouldBe(exception7);
        aggregateException.InnerExceptions[7].ShouldBe(exception8);
        aggregateException.InnerExceptions[8].ShouldBe(exception9);
    }
}