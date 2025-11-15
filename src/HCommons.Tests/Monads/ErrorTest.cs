using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Error))]
public class ErrorTest {
    
    [Fact]
    public void Constructor_WithMessage_CreatesErrorWithMessage() {
        var error = new Error("Test message");
        
        error.Message.ShouldBe("Test message");
        error.HasException.ShouldBeFalse();
        error.Exception.ShouldBeNull();
    }

    [Fact]
    public void Constructor_WithMessageAndException_CreatesErrorWithBoth() {
        var exception = new InvalidOperationException("Test exception");
        var error = new Error("Test message", exception);
        
        error.Message.ShouldBe("Test message");
        error.HasException.ShouldBeTrue();
        error.Exception.ShouldBe(exception);
    }

    [Fact]
    public void Empty_ReturnsErrorWithEmptyMessage() {
        var error = Error.Empty;
        
        error.Message.ShouldBeEmpty();
        error.HasException.ShouldBeFalse();
    }

    [Fact]
    public void FromException_CreatesErrorFromException() {
        var exception = new ArgumentException("Argument error");
        var error = Error.FromException(exception);
        
        error.Message.ShouldBe("Argument error");
        error.Exception.ShouldBe(exception);
        error.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithMessage_CreatesErrorWithMessage() {
        var error = Error.WithMessage("Custom message");
        
        error.Message.ShouldBe("Custom message");
        error.HasException.ShouldBeFalse();
    }

    [Fact]
    public void ImplicitConversion_FromException_CreatesError() {
        var exception = new InvalidOperationException("Operation failed");
        Error error = exception;
        
        error.Message.ShouldBe("Operation failed");
        error.Exception.ShouldBe(exception);
    }

    [Fact]
    public void ImplicitConversion_FromString_CreatesError() {
        Error error = "Error message";
        
        error.Message.ShouldBe("Error message");
        error.HasException.ShouldBeFalse();
    }

    [Fact]
    public void ToString_WithoutException_ReturnsFormattedMessage() {
        var error = new Error("Test message");
        
        error.ToString().ShouldBe("[Error]: Test message");
    }

    [Fact]
    public void ToString_WithException_IncludesExceptionInOutput() {
        var exception = new InvalidOperationException("Operation failed");
        var error = new Error("Test message", exception);
        var result = error.ToString();
        
        result.ShouldStartWith("[Error]: Test message");
        result.ShouldContain("| -> [Exception]:");
        result.ShouldContain("Operation failed");
    }

    [Fact]
    public void HasException_WithException_ReturnsTrue() {
        var error = new Error("Message", new Exception());
        
        error.HasException.ShouldBeTrue();
    }

    [Fact]
    public void HasException_WithoutException_ReturnsFalse() {
        var error = new Error("Message");
        
        error.HasException.ShouldBeFalse();
    }
}