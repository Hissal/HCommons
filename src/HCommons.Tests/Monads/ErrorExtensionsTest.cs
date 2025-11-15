using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(ErrorExtensions))]
public class ErrorExtensionsTest {

    [Fact]
    public void WithValue_AddsValueToError() {
        var error = new Error("Test message");
        var errorWithValue = error.WithValue(42);
        
        errorWithValue.Value.ShouldBe(42);
        errorWithValue.Message.ShouldBe("Test message");
        errorWithValue.HasException.ShouldBeFalse();
    }

    [Fact]
    public void WithValue_PreservesException() {
        var exception = new InvalidOperationException("Test exception");
        var error = new Error("Test message", exception);
        var errorWithValue = error.WithValue("test value");
        
        errorWithValue.Value.ShouldBe("test value");
        errorWithValue.Message.ShouldBe("Test message");
        errorWithValue.Exception.ShouldBe(exception);
        errorWithValue.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithValue_WithDifferentTypes_CreatesCorrectErrorType() {
        var error = new Error("Test message");
        
        var intError = error.WithValue(42);
        var stringError = error.WithValue("test");
        var listError = error.WithValue(new List<int> { 1, 2, 3 });
        
        intError.Value.ShouldBe(42);
        stringError.Value.ShouldBe("test");
        listError.Value.Count.ShouldBe(3);
    }

    [Fact]
    public void WithValue_WithNullValue_AllowsNull() {
        var error = new Error("Test message");
        var errorWithValue = error.WithValue<string?>(null);
        
        errorWithValue.Value.ShouldBeNull();
        errorWithValue.Message.ShouldBe("Test message");
    }

    [Fact]
    public void WithoutValue_RemovesValueFromError() {
        var errorWithValue = new Error<int>(42, "Test message");
        var error = errorWithValue.WithoutValue();
        
        error.Message.ShouldBe("Test message");
        error.HasException.ShouldBeFalse();
    }

    [Fact]
    public void WithoutValue_PreservesException() {
        var exception = new InvalidOperationException("Test exception");
        var errorWithValue = new Error<string>("test", "Test message", exception);
        var error = errorWithValue.WithoutValue();
        
        error.Message.ShouldBe("Test message");
        error.Exception.ShouldBe(exception);
        error.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithMessage_ChangesMessageOnError() {
        var error = new Error("Original message");
        var updated = error.WithMessage("New message");
        
        updated.Message.ShouldBe("New message");
        updated.HasException.ShouldBeFalse();
    }

    [Fact]
    public void WithMessage_PreservesExceptionOnError() {
        var exception = new InvalidOperationException("Test exception");
        var error = new Error("Original message", exception);
        var updated = error.WithMessage("New message");
        
        updated.Message.ShouldBe("New message");
        updated.Exception.ShouldBe(exception);
        updated.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithMessage_ChangesMessageOnErrorWithValue() {
        var error = new Error<int>(42, "Original message");
        var updated = error.WithMessage("New message");
        
        updated.Value.ShouldBe(42);
        updated.Message.ShouldBe("New message");
        updated.HasException.ShouldBeFalse();
    }

    [Fact]
    public void WithMessage_PreservesValueAndExceptionOnErrorWithValue() {
        var exception = new InvalidOperationException("Test exception");
        var error = new Error<string>("test value", "Original message", exception);
        var updated = error.WithMessage("New message");
        
        updated.Value.ShouldBe("test value");
        updated.Message.ShouldBe("New message");
        updated.Exception.ShouldBe(exception);
        updated.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithMessage_WithEmptyString_SetsEmptyMessage() {
        var error = new Error<int>(42, "Original message");
        var updated = error.WithMessage(string.Empty);
        
        updated.Message.ShouldBeEmpty();
        updated.Value.ShouldBe(42);
    }

    [Fact]
    public void WithException_AddsExceptionToError() {
        var error = new Error("Test message");
        var exception = new InvalidOperationException("New exception");
        var updated = error.WithException(exception);
        
        updated.Message.ShouldBe("Test message");
        updated.Exception.ShouldBe(exception);
        updated.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithException_ReplacesExistingExceptionOnError() {
        var oldException = new ArgumentException("Old exception");
        var newException = new InvalidOperationException("New exception");
        var error = new Error("Test message", oldException);
        var updated = error.WithException(newException);
        
        updated.Message.ShouldBe("Test message");
        updated.Exception.ShouldBe(newException);
        updated.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithException_AddsExceptionToErrorWithValue() {
        var error = new Error<int>(42, "Test message");
        var exception = new InvalidOperationException("New exception");
        var updated = error.WithException(exception);
        
        updated.Value.ShouldBe(42);
        updated.Message.ShouldBe("Test message");
        updated.Exception.ShouldBe(exception);
        updated.HasException.ShouldBeTrue();
    }

    [Fact]
    public void WithException_ReplacesExistingExceptionOnErrorWithValue() {
        var oldException = new ArgumentException("Old exception");
        var newException = new InvalidOperationException("New exception");
        var error = new Error<string>("test", "Test message", oldException);
        var updated = error.WithException(newException);
        
        updated.Value.ShouldBe("test");
        updated.Message.ShouldBe("Test message");
        updated.Exception.ShouldBe(newException);
        updated.HasException.ShouldBeTrue();
    }

    [Fact]
    public void ChainedOperations_WithValueThenWithMessage_WorksCorrectly() {
        var error = new Error("Original message");
        var updated = error.WithValue(42).WithMessage("New message");
        
        updated.Value.ShouldBe(42);
        updated.Message.ShouldBe("New message");
    }

    [Fact]
    public void ChainedOperations_WithValueThenWithException_WorksCorrectly() {
        var error = new Error("Test message");
        var exception = new InvalidOperationException("Test exception");
        var updated = error.WithValue(42).WithException(exception);
        
        updated.Value.ShouldBe(42);
        updated.Message.ShouldBe("Test message");
        updated.Exception.ShouldBe(exception);
    }

    [Fact]
    public void ChainedOperations_WithValueThenWithoutValue_ReturnsToOriginalError() {
        var error = new Error("Test message");
        var updated = error.WithValue(42).WithoutValue();
        
        updated.Message.ShouldBe("Test message");
        updated.HasException.ShouldBeFalse();
    }

    [Fact]
    public void ChainedOperations_AllExtensions_WorksCorrectly() {
        var error = new Error("Original");
        var exception = new InvalidOperationException("Test");
        
        var updated = error
            .WithValue(42)
            .WithMessage("Updated")
            .WithException(exception);
        
        updated.Value.ShouldBe(42);
        updated.Message.ShouldBe("Updated");
        updated.Exception.ShouldBe(exception);
    }

    [Fact]
    public void WithValue_FromEmptyError_CreatesErrorWithValue() {
        var error = Error.Empty;
        var errorWithValue = error.WithValue(100);
        
        errorWithValue.Value.ShouldBe(100);
        errorWithValue.Message.ShouldBeEmpty();
    }

    [Fact]
    public void WithMessage_WithLongMessage_HandlesCorrectly() {
        var longMessage = new string('a', 1000);
        var error = new Error<int>(42, "Short");
        var updated = error.WithMessage(longMessage);
        
        updated.Message.ShouldBe(longMessage);
        updated.Value.ShouldBe(42);
    }

    [Fact]
    public void WithException_WithNestedExceptions_PreservesInnerException() {
        var innerException = new ArgumentException("Inner");
        var outerException = new InvalidOperationException("Outer", innerException);
        var error = new Error<int>(42, "Test");
        var updated = error.WithException(outerException);
        
        updated.HasException.ShouldBeTrue();
        updated.Exception.ShouldBe(outerException);
        updated.Exception.InnerException.ShouldBe(innerException);
    }

    [Fact]
    public void WithValue_WithComplexType_StoresCorrectly() {
        var error = new Error("Test message");
        var complexValue = new { Id = 1, Name = "Test", Items = new List<int> { 1, 2, 3 } };
        var errorWithValue = error.WithValue(complexValue);
        
        errorWithValue.Value.Id.ShouldBe(1);
        errorWithValue.Value.Name.ShouldBe("Test");
        errorWithValue.Value.Items.Count.ShouldBe(3);
    }
}