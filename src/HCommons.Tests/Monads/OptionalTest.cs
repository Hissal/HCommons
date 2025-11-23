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