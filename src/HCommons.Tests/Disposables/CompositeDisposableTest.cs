using HCommons.Disposables;

namespace HCommons.Tests.Disposables;

[TestSubject(typeof(CompositeDisposable))]
public class CompositeDisposableTest {
    [Fact]
    public void Ctor_NegativeCapacity_Throws() {
        var ex = Should.Throw<ArgumentOutOfRangeException>(() => new CompositeDisposable(-1));
        ex.ParamName.ShouldBe("capacity");
    }

    [Fact]
    public void Ctor_WithCapacity_InitialCountIsZero() {
        var sut = new CompositeDisposable(4);
        sut.Count.ShouldBe(0);
    }

    [Fact]
    public void Ctor_WithParams_AddsAllItems() {
        // Arrange
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();

        // Act
        var sut = new CompositeDisposable(d1, d2);

        // Assert
        sut.Count.ShouldBe(2);
        sut.Contains(d1).ShouldBeTrue();
        sut.Contains(d2).ShouldBeTrue();
    }

    [Fact]
    public void Ctor_WithEnumerable_AddsAllItems() {
        // Arrange
        var disposables = Enumerable.Range(0, 10).Select(x => Substitute.For<IDisposable>());
        var enumerable = disposables.ToList();

        // Act
        var sut = new CompositeDisposable(enumerable);

        // Assert
        sut.Count.ShouldBe(10);
        Assert.All(enumerable, x => sut.Contains(x).ShouldBeTrue());
    }

    [Fact]
    public void Add_AddsItem_IncrementsCount() {
        // Arrange
        var sut = new CompositeDisposable();
        var d1 = Substitute.For<IDisposable>();

        // Act
        sut.Add(d1);

        // Assert
        sut.Count.ShouldBe(1);
        sut.Contains(d1).ShouldBeTrue();
    }

    [Fact]
    public void Add_WhenDisposed_DisposesImmediately() {
        // Arrange
        var sut = new CompositeDisposable();
        var d = Substitute.For<IDisposable>();

        // Act
        sut.Dispose();
        sut.Add(d);

        // Assert
        d.Received(1).Dispose();
        sut.Count.ShouldBe(0);
    }

    [Fact]
    public void Clear_DisposesAllAndEmptiesCollection() {
        // Arrange
        var sut = new CompositeDisposable();
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        sut.Add(d1);
        sut.Add(d2);

        // Act
        sut.Clear();

        // Assert
        sut.Count.ShouldBe(0);
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
        sut.IsDisposed.ShouldBeFalse();
    }

    [Fact]
    public void Clear_OnEmpty_DoesNothing() {
        // Arrange
        var sut = new CompositeDisposable();

        // Act
        sut.Clear();

        // Assert
        sut.Count.ShouldBe(0);
        sut.IsDisposed.ShouldBeFalse();
    }

    [Fact]
    public void Dispose_DisposesAllAndSetsIsDisposed() {
        // Arrange
        var sut = new CompositeDisposable();
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        sut.Add(d1);
        sut.Add(d2);

        // Act
        sut.Dispose();

        // Assert
        sut.IsDisposed.ShouldBeTrue();
        d1.Received(1).Dispose();
        d2.Received(1).Dispose();
        sut.Count.ShouldBe(0);
    }

    [Fact]
    public void Dispose_IsIdempotent() {
        // Arrange
        var sut = new CompositeDisposable();
        var d = Substitute.For<IDisposable>();
        sut.Add(d);

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        d.Received(1).Dispose();
        sut.IsDisposed.ShouldBeTrue();
    }

    [Fact]
    public void Remove_RemovesItem_AndDoesNotDisposeIt() {
        // Arrange
        var sut = new CompositeDisposable();
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        sut.Add(d1);
        sut.Add(d2);

        // Act
        var removed = sut.Remove(d1);

        // Assert
        removed.ShouldBeTrue();
        sut.Count.ShouldBe(1);
        sut.Contains(d1).ShouldBeFalse();
        sut.Contains(d2).ShouldBeTrue();
        d1.DidNotReceive().Dispose();
    }

    [Fact]
    public void Remove_MissingItem_ReturnsFalse() {
        // Arrange
        var sut = new CompositeDisposable();
        var d = Substitute.For<IDisposable>();

        // Act
        var removed = sut.Remove(d);

        // Assert
        removed.ShouldBeFalse();
        sut.Count.ShouldBe(0);
    }

    [Fact]
    public void Contains_ReturnsTrueForExistingItem() {
        // Arrange
        var sut = new CompositeDisposable();
        var d = Substitute.For<IDisposable>();
        sut.Add(d);
        
        // Assert
        sut.Contains(d).ShouldBeTrue();
    }

    [Fact]
    public void IsReadOnly_IsFalse() {
        var sut = new CompositeDisposable();
        sut.IsReadOnly.ShouldBeFalse();
    }

    [Fact]
    public void CopyTo_CopiesItemsInOrderStartingAtIndex() {
        // Arrange
        var sut = new CompositeDisposable();
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        var d3 = Substitute.For<IDisposable>();
        sut.Add(d1);
        sut.Add(d2);
        sut.Add(d3);
        var target = new IDisposable[5];

        // Act
        sut.CopyTo(target, 1);

        // Assert
        target[0].ShouldBeNull();
        target[1].ShouldBe(d1);
        target[2].ShouldBe(d2);
        target[3].ShouldBe(d3);
        target[4].ShouldBeNull();
    }

    [Fact]
    public void Enumerator_EnumeratesAllItemsInOrder() {
        // Arrange
        var sut = new CompositeDisposable();
        var d1 = Substitute.For<IDisposable>();
        var d2 = Substitute.For<IDisposable>();
        sut.Add(d1);
        sut.Add(d2);

        // Act
        var items = sut.ToArray();

        // Assert
        items.ShouldBeEquivalentTo(new[] {d1, d2});
    }
}