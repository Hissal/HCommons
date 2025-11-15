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
    public void Bind_WhenHasValue_TransformsValue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Bind(value => Optional<string>.Some(value.ToString()));
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe("42");
    }

    [Fact]
    public void Bind_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Bind(value => Optional<string>.Some(value.ToString()));
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Bind_WhenHasValue_CanReturnNone() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Bind(_ => Optional<string>.None());
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Bind_WithState_WhenHasValue_TransformsValue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Bind(
            10,
            (state, value) => Optional<int>.Some(value + state)
        );
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(52);
    }

    [Fact]
    public void Bind_WithState_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Bind(
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

    [Fact]
    public void Select_WhenHasValue_TransformsValue() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Select(x => x.ToString());
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe("42");
    }

    [Fact]
    public void Select_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Select(x => x.ToString());
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Select_WithState_WhenHasValue_TransformsValue() {
        var optional = Optional<int>.Some(10);
        
        var result = optional.Select(5, (state, value) => value * state);
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(50);
    }

    [Fact]
    public void Select_WithState_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Select(5, (state, value) => value * state);
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void SelectMany_WhenHasValue_AndInnerHasValue_CombinesValues() {
        var optional = Optional<int>.Some(10);
        
        var result = optional.SelectMany(
            x => Optional<string>.Some(x.ToString()),
            (outer, inner) => $"{outer}-{inner}"
        );
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe("10-10");
    }

    [Fact]
    public void SelectMany_WhenHasValue_AndInnerIsNone_ReturnsNone() {
        var optional = Optional<int>.Some(10);
        
        var result = optional.SelectMany(
            x => Optional<string>.None(),
            (outer, inner) => $"{outer}-{inner}"
        );
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void SelectMany_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.SelectMany(
            x => Optional<string>.Some(x.ToString()),
            (outer, inner) => $"{outer}-{inner}"
        );
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void SelectMany_WithState_WhenHasValue_AndInnerHasValue_CombinesValues() {
        var optional = Optional<int>.Some(10);
        
        var result = optional.SelectMany(
            100,
            (state, x) => Optional<int>.Some(x + state),
            (state, outer, inner) => state + outer + inner
        );
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(220);
    }

    [Fact]
    public void SelectMany_WithState_WhenHasValue_AndInnerIsNone_ReturnsNone() {
        var optional = Optional<int>.Some(10);
        
        var result = optional.SelectMany(
            100,
            (state, x) => Optional<int>.None(),
            (state, outer, inner) => state + outer + inner
        );
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void SelectMany_WithState_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.SelectMany(
            100,
            (state, x) => Optional<int>.Some(x + state),
            (state, outer, inner) => state + outer + inner
        );
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Where_WhenHasValue_AndPredicateIsTrue_ReturnsSameOptional() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Where(x => x > 40);
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Where_WhenHasValue_AndPredicateIsFalse_ReturnsNone() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Where(x => x > 50);
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Where_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Where(x => x > 40);
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Where_WithState_WhenHasValue_AndPredicateIsTrue_ReturnsSameOptional() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Where(40, (state, value) => value > state);
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Where_WithState_WhenHasValue_AndPredicateIsFalse_ReturnsNone() {
        var optional = Optional<int>.Some(42);
        
        var result = optional.Where(50, (state, value) => value > state);
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void Where_WithState_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var result = optional.Where(40, (state, value) => value > state);
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void LinqQuery_WhenAllHaveValues_CombinesCorrectly() {
        var optional1 = Optional<int>.Some(10);
        var optional2 = Optional<int>.Some(20);
        
        var result = from x in optional1
                     from y in optional2
                     where x + y > 20
                     select x + y;
        
        result.HasValue.ShouldBeTrue();
        result.Value.ShouldBe(30);
    }

    [Fact]
    public void LinqQuery_WhenFirstIsNone_ReturnsNone() {
        var optional1 = Optional<int>.None();
        var optional2 = Optional<int>.Some(20);
        
        var result = from x in optional1
                     from y in optional2
                     select x + y;
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void LinqQuery_WhenSecondIsNone_ReturnsNone() {
        var optional1 = Optional<int>.Some(10);
        var optional2 = Optional<int>.None();
        
        var result = from x in optional1
                     from y in optional2
                     select x + y;
        
        result.HasValue.ShouldBeFalse();
    }

    [Fact]
    public void LinqQuery_WhenWhereFilterFails_ReturnsNone() {
        var optional1 = Optional<int>.Some(10);
        var optional2 = Optional<int>.Some(20);
        
        var result = from x in optional1
                     from y in optional2
                     where x + y > 50
                     select x + y;
        
        result.HasValue.ShouldBeFalse();
    }
}