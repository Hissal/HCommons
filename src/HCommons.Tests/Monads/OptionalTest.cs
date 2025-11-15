using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Optional<>))]
public class OptionalTest {

    [Fact]
    public void Some_CreatesOptionalWithValue() {
        var optional = Optional<int>.Some(42);
        
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public void Some_SetsHasValueToTrue() {
        var optional = Optional<int>.Some(42);
        
        optional.HasValue.ShouldBeTrue();
    }

    [Fact]
    public void None_CreatesOptionalWithoutValue() {
        var optional = Optional<int>.None();
        
        optional.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSome() {
        Optional<int> optional = 42;
        
        optional.Value.ShouldBe(42);
        optional.HasValue.ShouldBeTrue();
    }

    [Fact]
    public void TryGetValue_WhenHasValue_ReturnsTrue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.TryGetValue(out var value);
        
        result.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetValue_WhenNone_ReturnsFalse() {
        var optional = Optional<int>.None();
        
        var result = optional.TryGetValue(out var value);
        
        result.ShouldBeFalse();
    }

    [Fact]
    public void GetValueOrDefault_WhenHasValue_ReturnsValue() {
        var optional = Optional<int>.Some(42);
        
        optional.GetValueOrDefault().ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_WhenNone_ReturnsDefault() {
        var optional = Optional<int>.None();
        
        optional.GetValueOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_WhenHasValue_ReturnsValue() {
        var optional = Optional<int>.Some(42);
        
        optional.GetValueOrDefault(100).ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_WhenNone_ReturnsDefaultValue() {
        var optional = Optional<int>.None();
        
        optional.GetValueOrDefault(100).ShouldBe(100);
    }

    [Fact]
    public void Match_WhenHasValue_ExecutesOnValue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Match(
            value => value * 2,
            () => 0
        );
        
        result.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenNone_ExecutesOnNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Match(
            value => value * 2,
            () => 0
        );
        
        result.ShouldBe(0);
    }

    [Fact]
    public void Match_WithState_WhenHasValue_ExecutesOnValue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Match(
            10,
            (state, value) => value + state,
            state => state
        );
        
        result.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenNone_ExecutesOnNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Match(
            10,
            (state, value) => value + state,
            state => state
        );
        
        result.ShouldBe(10);
    }

    [Fact]
    public void Select_WhenHasValue_TransformsValue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Select(value => Optional<string>.Some(value.ToString()));
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe("42");
    }

    [Fact]
    public void Select_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Select(value => Optional<string>.Some(value.ToString()));
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Select_WhenHasValue_CanReturnNone() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Select(_ => Optional<string>.None());
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Select_WithState_WhenHasValue_TransformsValue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Select(
            10,
            (state, value) => Optional<int>.Some(value + state)
        );
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(52);
    }

    [Fact]
    public void Select_WithState_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Select(
            10,
            (state, value) => Optional<int>.Some(value + state)
        );
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenHasValue_ExecutesOnValue() {
        var optional = Optional<int>.Some(42);
        var valueCalled = false;
        var noneCalled = false;
        
        optional.Switch(
            value => valueCalled = true,
            () => noneCalled = true
        );
        
        valueCalled.ShouldBeTrue();
        noneCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenNone_ExecutesOnNone() {
        var optional = Optional<int>.None();
        var valueCalled = false;
        var noneCalled = false;
        
        optional.Switch(
            value => valueCalled = true,
            () => noneCalled = true
        );
        
        valueCalled.ShouldBeFalse();
        noneCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenHasValue_ExecutesOnValue() {
        var optional = Optional<int>.Some(42);
        var counter = 0;
        
        optional.Switch(
            100,
            (state, value) => counter = state + value,
            state => counter = state
        );
        
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenNone_ExecutesOnNone() {
        var optional = Optional<int>.None();
        var counter = 0;
        
        optional.Switch(
            100,
            (state, value) => counter = state + value,
            state => counter = state
        );
        
        counter.ShouldBe(100);
    }

    [Fact]
    public void ToString_WhenHasValue_ReturnsFormattedString() {
        var optional = Optional<int>.Some(42);
        
        optional.ToString().ShouldBe("Some: 42");
    }

    [Fact]
    public void ToString_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        optional.ToString().ShouldBe("None");
    }

    [Fact]
    public void Some_WithReferenceType_StoresReference() {
        var list = new List<int> { 1, 2, 3 };
        var optional = Optional<List<int>>.Some(list);
        
        optional.Value.ShouldBeSameAs(list);
    }
}