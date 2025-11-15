using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Error<>))]
public class ErrorTest_TValue {

    [Fact]
    public void Constructor_WithValueAndMessage_CreatesErrorWithBoth() {
        var error = new Error<int>(42, "Test message");
        
        error.Value.ShouldBe(42);
        error.Message.ShouldBe("Test message");
        error.HasException.ShouldBeFalse();
        error.Exception.ShouldBeNull();
    }

    [Fact]
    public void Constructor_WithValueMessageAndException_CreatesErrorWithAll() {
        var exception = new InvalidOperationException("Test exception");
        var error = new Error<string>("test value", "Test message", exception);
        
        error.Value.ShouldBe("test value");
        error.Message.ShouldBe("Test message");
        error.HasException.ShouldBeTrue();
        error.Exception.ShouldBe(exception);
    }

    [Fact]
    public void FromException_CreatesErrorFromException() {
        var exception = new ArgumentException("Argument error");
        var error = Error<int>.FromException(100, exception);
        
        error.Value.ShouldBe(100);
        error.Message.ShouldBe("Argument error");
        error.Exception.ShouldBe(exception);
        error.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithMessage_CreatesErrorWithValueAndMessage() {
        var error = Error<string>.WithMessage("test", "Custom message");
        
        error.Value.ShouldBe("test");
        error.Message.ShouldBe("Custom message");
        error.HasException.ShouldBeFalse();
    }

    [Fact]
    public void ValueOnly_CreatesErrorWithValueAndEmptyMessage() {
        var error = Error<int>.ValueOnly(42);
        
        error.Value.ShouldBe(42);
        error.Message.ShouldBeEmpty();
        error.HasException.ShouldBeFalse();
    }

    [Fact]
    public void ImplicitConversion_ToError_PreservesMessageAndException() {
        var exception = new InvalidOperationException("Operation failed");
        var errorWithValue = new Error<int>(42, "Test message", exception);
        Error error = errorWithValue;
        
        error.Message.ShouldBe("Test message");
        error.Exception.ShouldBe(exception);
        error.HasException.ShouldBeTrue();
    }

    [Fact]
    public void ToString_WithoutException_ReturnsFormattedString() {
        var error = new Error<int>(42, "Test message");
        var result = error.ToString();
        
        result.ShouldBe("[Error]: Test message | -> [Value]: 42");
    }

    [Fact]
    public void ToString_WithException_IncludesExceptionInOutput() {
        var exception = new InvalidOperationException("Operation failed");
        var error = new Error<string>("test", "Test message", exception);
        var result = error.ToString();
        
        result.ShouldStartWith("[Error]: Test message");
        result.ShouldContain("| -> [Exception]:");
        result.ShouldContain("Operation failed");
        result.ShouldContain("| -> [Value]: test");
    }

    [Fact]
    public void HasException_WithException_ReturnsTrue() {
        var error = new Error<int>(42, "Message", new Exception());
        
        error.HasException.ShouldBeTrue();
    }

    [Fact]
    public void HasException_WithoutException_ReturnsFalse() {
        var error = new Error<int>(42, "Message");
        
        error.HasException.ShouldBeFalse();
    }

    [Fact]
    public void Constructor_WithReferenceType_StoresReference() {
        var list = new List<int> { 1, 2, 3 };
        var error = new Error<List<int>>(list, "Test message");
        
        error.Value.ShouldBeSameAs(list);
        error.Value.Count.ShouldBe(3);
    }

    [Fact]
    public void ToString_WithNullValue_HandlesNull() {
        var error = new Error<string?>(null, "Test message");
        var result = error.ToString();
        
        result.ShouldContain("[Error]: Test message");
        result.ShouldContain("| -> [Value]:");
    }
}

