using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(EitherExtensions))]
public class EitherExtensionsTest {

    [Fact]
    public void Swap_WhenLeft_ReturnsRight() {
        var either = Either<int, string>.FromLeft(42);
        var swapped = either.Swap();
        swapped.IsRight.ShouldBeTrue();
        swapped.Right.ShouldBe(42);
    }

    [Fact]
    public void Swap_WhenRight_ReturnsLeft() {
        var either = Either<int, string>.FromRight("test");
        var swapped = either.Swap();
        swapped.IsLeft.ShouldBeTrue();
        swapped.Left.ShouldBe("test");
    }

    [Fact]
    public void AsLeftOptional_WhenLeft_ReturnsSome() {
        var either = Either<int, string>.FromLeft(42);
        var optional = either.AsLeftOptional();
        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void AsLeftOptional_WhenRight_ReturnsNone() {
        var either = Either<int, string>.FromRight("test");
        var optional = either.AsLeftOptional();
        optional.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void AsRightOptional_WhenRight_ReturnsSome() {
        var either = Either<int, string>.FromRight("test");
        var optional = either.AsRightOptional();
        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public void AsRightOptional_WhenLeft_ReturnsNone() {
        var either = Either<int, string>.FromLeft(42);
        var optional = either.AsRightOptional();
        optional.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Map_WhenLeft_TransformsBothSides() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = either.Map(x => x * 2, s => s.Length);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public void Map_WhenRight_TransformsBothSides() {
        var either = Either<int, string>.FromRight("test");
        var mapped = either.Map(x => x * 2, s => s.Length);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public void Map_WithState_WhenLeft_TransformsLeft() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = either.Map(10, (state, x) => x + state, (state, s) => s.Length + state);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(52);
    }

    [Fact]
    public void Map_WithState_WhenRight_TransformsRight() {
        var either = Either<int, string>.FromRight("test");
        var mapped = either.Map(10, (state, x) => x + state, (state, s) => s.Length + state);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(14);
    }

    [Fact]
    public void MapLeft_WhenLeft_TransformsValue() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = either.MapLeft(x => x * 2);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public void MapLeft_WhenRight_DoesNotTransform() {
        var either = Either<int, string>.FromRight("test");
        var mapped = either.MapLeft(x => x * 2);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe("test");
    }

    [Fact]
    public void MapLeft_WithState_WhenLeft_TransformsValue() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = either.MapLeft(10, (state, x) => x + state);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(52);
    }

    [Fact]
    public void MapLeft_WithState_WhenRight_DoesNotTransform() {
        var either = Either<int, string>.FromRight("test");
        var mapped = either.MapLeft(10, (state, x) => x + state);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe("test");
    }

    [Fact]
    public void MapRight_WhenRight_TransformsValue() {
        var either = Either<int, string>.FromRight("test");
        var mapped = either.MapRight(s => s.Length);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public void MapRight_WhenLeft_DoesNotTransform() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = either.MapRight(s => s.Length);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(42);
    }

    [Fact]
    public void MapRight_WithState_WhenRight_TransformsValue() {
        var either = Either<int, string>.FromRight("test");
        var mapped = either.MapRight(10, (state, s) => s.Length + state);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(14);
    }

    [Fact]
    public void MapRight_WithState_WhenLeft_DoesNotTransform() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = either.MapRight(10, (state, s) => s.Length + state);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(42);
    }

    [Fact]
    public void Match_WhenLeft_ExecutesLeftFunc() {
        var either = Either<int, string>.FromLeft(42);
        var result = either.Match(x => x * 2, s => s.Length);
        result.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenRight_ExecutesRightFunc() {
        var either = Either<int, string>.FromRight("test");
        var result = either.Match(x => x * 2, s => s.Length);
        result.ShouldBe(4);
    }

    [Fact]
    public void Match_WithState_WhenLeft_ExecutesLeftFunc() {
        var either = Either<int, string>.FromLeft(42);
        var result = either.Match(10, (state, x) => x + state, (state, s) => s.Length + state);
        result.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenRight_ExecutesRightFunc() {
        var either = Either<int, string>.FromRight("test");
        var result = either.Match(10, (state, x) => x + state, (state, s) => s.Length + state);
        result.ShouldBe(14);
    }

    [Fact]
    public void Switch_WhenLeft_ExecutesLeftAction() {
        var either = Either<int, string>.FromLeft(42);
        var leftCalled = false;
        var rightCalled = false;
        either.Switch(_ => leftCalled = true, _ => rightCalled = true);
        leftCalled.ShouldBeTrue();
        rightCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenRight_ExecutesRightAction() {
        var either = Either<int, string>.FromRight("test");
        var leftCalled = false;
        var rightCalled = false;
        either.Switch(_ => leftCalled = true, _ => rightCalled = true);
        leftCalled.ShouldBeFalse();
        rightCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenLeft_ExecutesLeftAction() {
        var either = Either<int, string>.FromLeft(42);
        var counter = 0;
        either.Switch(100, (state, x) => counter = state + x, (state, s) => counter = state + s.Length);
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenRight_ExecutesRightAction() {
        var either = Either<int, string>.FromRight("test");
        var counter = 0;
        either.Switch(100, (state, x) => counter = state + x, (state, s) => counter = state + s.Length);
        counter.ShouldBe(104);
    }

    [Fact]
    public void Map_ChainedOperations_WorksCorrectly() {
        var either = Either<int, string>.FromLeft(10);
        var result = either
            .MapLeft(x => x * 2)
            .MapLeft(x => x + 5);
        result.IsLeft.ShouldBeTrue();
        result.Left.ShouldBe(25);
    }

    [Fact]
    public void Swap_TwiceReturnsOriginal() {
        var either = Either<int, string>.FromLeft(42);
        var swapped = either.Swap().Swap();
        swapped.IsLeft.ShouldBeTrue();
        swapped.Left.ShouldBe(42);
    }
}

