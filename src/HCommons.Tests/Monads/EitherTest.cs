using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Either<,>))]
public class EitherTest {

    [Fact]
    public void FromLeft_CreatesLeftEither() {
        var either = Either<int, string>.FromLeft(42);
        
        either.Left.ShouldBe(42);
    }

    [Fact]
    public void FromLeft_SetsTypeToLeft() {
        var either = Either<int, string>.FromLeft(42);
        
        either.Type.ShouldBe(EitherType.Left);
        either.IsLeft.ShouldBeTrue();
        either.IsRight.ShouldBeFalse();
    }

    [Fact]
    public void FromRight_CreatesRightEither() {
        var either = Either<int, string>.FromRight("test");
        
        either.Right.ShouldBe("test");
    }

    [Fact]
    public void FromRight_SetsTypeToRight() {
        var either = Either<int, string>.FromRight("test");
        
        either.Type.ShouldBe(EitherType.Right);
        either.IsRight.ShouldBeTrue();
        either.IsLeft.ShouldBeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromLeft_CreatesLeftEither() {
        Either<int, string> either = 42;
        
        either.Left.ShouldBe(42);
        either.IsLeft.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromRight_CreatesRightEither() {
        Either<int, string> either = "test";
        
        either.Right.ShouldBe("test");
        either.IsRight.ShouldBeTrue();
    }

    [Fact]
    public void TryGetLeft_WhenLeft_ReturnsTrue() {
        var either = Either<int, string>.FromLeft(42);
        
        var result = either.TryGetLeft(out var value);
        
        result.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetLeft_WhenRight_ReturnsFalse() {
        var either = Either<int, string>.FromRight("test");
        
        var result = either.TryGetLeft(out _);
        
        result.ShouldBeFalse();
    }

    [Fact]
    public void TryGetRight_WhenRight_ReturnsTrue() {
        var either = Either<int, string>.FromRight("test");
        
        var result = either.TryGetRight(out var value);
        
        result.ShouldBeTrue();
        value.ShouldBe("test");
    }

    [Fact]
    public void TryGetRight_WhenLeft_ReturnsFalse() {
        var either = Either<int, string>.FromLeft(42);
        
        var result = either.TryGetRight(out _);
        
        result.ShouldBeFalse();
    }

    [Fact]
    public void GetLeftOrDefault_WhenLeft_ReturnsValue() {
        var either = Either<int, string>.FromLeft(42);
        
        either.GetLeftOrDefault().ShouldBe(42);
    }

    [Fact]
    public void GetLeftOrDefault_WhenRight_ReturnsDefault() {
        var either = Either<int, string>.FromRight("test");
        
        either.GetLeftOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetLeftOrDefault_WithDefaultValue_WhenLeft_ReturnsValue() {
        var either = Either<int, string>.FromLeft(42);
        
        either.GetLeftOrDefault(100).ShouldBe(42);
    }

    [Fact]
    public void GetLeftOrDefault_WithDefaultValue_WhenRight_ReturnsDefaultValue() {
        var either = Either<int, string>.FromRight("test");
        
        either.GetLeftOrDefault(100).ShouldBe(100);
    }

    [Fact]
    public void GetRightOrDefault_WhenRight_ReturnsValue() {
        var either = Either<int, string>.FromRight("test");
        
        either.GetRightOrDefault().ShouldBe("test");
    }

    [Fact]
    public void GetRightOrDefault_WhenLeft_ReturnsDefault() {
        var either = Either<int, string>.FromLeft(42);
        
        either.GetRightOrDefault().ShouldBe(default(string));
    }

    [Fact]
    public void GetRightOrDefault_WithDefaultValue_WhenRight_ReturnsValue() {
        var either = Either<int, string>.FromRight("test");
        
        either.GetRightOrDefault("default").ShouldBe("test");
    }

    [Fact]
    public void GetRightOrDefault_WithDefaultValue_WhenLeft_ReturnsDefaultValue() {
        var either = Either<int, string>.FromLeft(42);
        
        either.GetRightOrDefault("default").ShouldBe("default");
    }

    [Fact]
    public void Match_WhenLeft_ExecutesLeftFunc() {
        var either = Either<int, string>.FromLeft(42);
        
        var result = either.Match(
            left => left * 2,
            right => right.Length
        );
        
        result.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenRight_ExecutesRightFunc() {
        var either = Either<int, string>.FromRight("test");
        
        var result = either.Match(
            left => left * 2,
            right => right.Length
        );
        
        result.ShouldBe(4);
    }

    [Fact]
    public void Match_WithState_WhenLeft_ExecutesLeftFunc() {
        var either = Either<int, string>.FromLeft(42);
        
        var result = either.Match(
            10,
            (state, left) => left + state,
            (state, right) => right.Length + state
        );
        
        result.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenRight_ExecutesRightFunc() {
        var either = Either<int, string>.FromRight("test");
        
        var result = either.Match(
            10,
            (state, left) => left + state,
            (state, right) => right.Length + state
        );
        
        result.ShouldBe(14);
    }

    [Fact]
    public void Switch_WhenLeft_ExecutesLeftAction() {
        var either = Either<int, string>.FromLeft(42);
        var leftCalled = false;
        var rightCalled = false;
        
        either.Switch(
            _ => leftCalled = true,
            _ => rightCalled = true
        );
        
        leftCalled.ShouldBeTrue();
        rightCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenRight_ExecutesRightAction() {
        var either = Either<int, string>.FromRight("test");
        var leftCalled = false;
        var rightCalled = false;
        
        either.Switch(
            _ => leftCalled = true,
            _ => rightCalled = true
        );
        
        leftCalled.ShouldBeFalse();
        rightCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenLeft_ExecutesLeftAction() {
        var either = Either<int, string>.FromLeft(42);
        var counter = 0;
        
        either.Switch(
            100,
            (state, left) => counter = state + left,
            (state, right) => counter = state + right.Length
        );
        
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenRight_ExecutesRightAction() {
        var either = Either<int, string>.FromRight("test");
        var counter = 0;
        
        either.Switch(
            100,
            (state, left) => counter = state + left,
            (state, right) => counter = state + right.Length
        );
        
        counter.ShouldBe(104);
    }

    [Fact]
    public void ToString_WhenLeft_ReturnsFormattedString() {
        var either = Either<int, string>.FromLeft(42);
        
        either.ToString().ShouldBe("Left: 42");
    }

    [Fact]
    public void ToString_WhenRight_ReturnsFormattedString() {
        var either = Either<int, string>.FromRight("test");
        
        either.ToString().ShouldBe("Right: test");
    }

    [Fact]
    public void Constructor_WithReferenceTypes_StoresCorrectly() {
        var list = new List<int> { 1, 2, 3 };
        var either = Either<List<int>, string>.FromLeft(list);
        
        either.Left.ShouldBeSameAs(list);
    }
}