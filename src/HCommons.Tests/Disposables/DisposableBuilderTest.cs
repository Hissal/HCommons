using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(DisposableBuilder))]
public class DisposableBuilderTest {
    [Fact]
    public void Build_NoItems_ReturnsEmpty() {
        var builder = new DisposableBuilder();
        var result = builder.Build();
        result.ShouldBeSameAs(Disposable.Empty);
    }

    [Fact]
    public void Build_SingleItem_ReturnsSameInstance() {
        // Arrange
        var builder = new DisposableBuilder();
        var d1 = Substitute.For<IDisposable>();
        builder.Add(d1);

        // Act
        var result = builder.Build();

        // Assert
        result.ShouldBeSameAs(d1);
    }

    [Fact]
    public void Build_TwoItems_DisposesBothWhenResultDisposed() {
        // Arrange
        var builder = new DisposableBuilder();
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        builder.Add(d1);
        builder.Add(d2);

        // Act
        var combined = builder.Build();
        combined.Dispose();

        // Assert
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
        combined.ShouldNotBeSameAs(d1);
        combined.ShouldNotBeSameAs(d2);
    }

    [Fact]
    public void Build_MoreThanEightItems_DisposesAllOnCombinedDispose() {
        // Arrange
        var builder = new DisposableBuilder();
        var items = Enumerable.Range(0, 12).Select(_ => Substitute.For<IDisposable>()).ToArray();
        foreach (var d in items) builder.Add(d);

        // Act
        var combined = builder.Build();
        combined.Dispose();

        // Assert
        foreach (var d in items) {
            d.Received(1).Dispose();
        }
    }

    [Fact]
    public void Add_AfterDispose_DisposesImmediately() {
        // Arrange
        var builder = new DisposableBuilder();
        var d = Substitute.For<IDisposable>();

        // Act
        builder.Dispose();
        builder.Add(d);

        // Assert
        d.Received(1).Dispose();
    }

    [Fact]
    public void Build_Twice_ThrowsObjectDisposedException() {
        Should.Throw<ObjectDisposedException>(() => {
            var builder = new DisposableBuilder();
            var d = Substitute.For<IDisposable>();
            builder.Add(d);
            
            builder.Build();
            builder.Build(); // Should throw
        });
    }

    [Fact]
    public void Build_AfterDispose_ThrowsObjectDisposedException() {
        Should.Throw<ObjectDisposedException>(() => {
            var builder = new DisposableBuilder();
            var d = Substitute.For<IDisposable>();
            builder.Add(d);
            
            builder.Dispose();
            builder.Build(); // Should throw
        });
    }

    [Fact]
    public void Build_DoesNotDisposeUnderlyingUntilResultDisposed() {
        // Arrange
        var builder = new DisposableBuilder();
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        builder.Add(d1);
        builder.Add(d2);

        // Act
        var combined = builder.Build();

        // Assert
        d1.DidNotReceive().Dispose();
        d2.DidNotReceive().Dispose();

        // Act
        combined.Dispose();

        // Assert
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
    }

    [Fact]
    public void IsDisposed_InitiallyFalse() {
        // Arrange
        var builder = new DisposableBuilder();

        // Assert
        builder.IsDisposed.ShouldBeFalse();
    }

    [Fact]
    public void IsDisposed_TrueAfterDispose() {
        // Arrange
        var builder = new DisposableBuilder();

        // Act
        builder.Dispose();

        // Assert
        builder.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void IsDisposed_TrueAfterBuild() {
        // Arrange
        var builder = new DisposableBuilder();
        builder.Add(Substitute.For<IDisposable>());

        // Act
        builder.Build();

        // Assert
        builder.IsDisposed.ShouldBeTrue();
    }
}