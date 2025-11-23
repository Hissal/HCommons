using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Result<>))]
public class Result1Test {

    [Fact]
    public void Success_CreatesSuccessResult() {
        var result = Result<int>.Success(42);
        
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Success_StoresValue() {
        var result = Result<int>.Success(42);
        
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Success_HasEmptyError() {
        var result = Result<int>.Success(42);
        
        result.Error.ShouldBe(Error.Empty);
    }

    [Fact]
    public void Failure_CreatesFailureResult() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Failure_StoresError() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ImplicitConversion_FromValue_CreatesSuccess() {
        Result<int> result = 42;
        
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure() {
        var error = new Error("Test error");
        Result<int> result = error;
        
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void TryGetValue_WhenSuccess_ReturnsTrue() {
        var result = Result<int>.Success(42);
        
        var success = result.TryGetValue(out var value);
        
        success.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetValue_WhenFailure_ReturnsFalse() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var success = result.TryGetValue(out _);
        
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryGetError_WhenFailure_ReturnsTrue() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        var hasError = result.TryGetError(out var resultError);
        
        hasError.ShouldBeTrue();
        resultError.ShouldBe(error);
    }

    [Fact]
    public void TryGetError_WhenSuccess_ReturnsFalse() {
        var result = Result<int>.Success(42);
        
        var hasError = result.TryGetError(out _);
        
        hasError.ShouldBeFalse();
    }

    [Fact]
    public void GetValueOrDefault_WhenSuccess_ReturnsValue() {
        var result = Result<int>.Success(42);
        
        result.GetValueOrDefault().ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_WhenFailure_ReturnsDefault() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        result.GetValueOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_WhenSuccess_ReturnsValue() {
        var result = Result<int>.Success(42);
        
        result.GetValueOrDefault(100).ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_WhenFailure_ReturnsDefaultValue() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        result.GetValueOrDefault(100).ShouldBe(100);
    }


    [Fact]
    public void ToString_WhenSuccess_ReturnsFormattedString() {
        var result = Result<int>.Success(42);
        
        result.ToString().ShouldBe("Success: 42");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFormattedString() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        result.ToString().ShouldStartWith("Failure:");
        result.ToString().ShouldContain("Test error");
    }

    [Fact]
    public void Success_WithReferenceType_StoresReference() {
        var list = new List<int> { 1, 2, 3 };
        var result = Result<List<int>>.Success(list);
        
        result.Value.ShouldBeSameAs(list);
    }

    [Fact]
    public void Failure_WithErrorWithException_PreservesException() {
        var exception = new InvalidOperationException("Operation failed");
        var error = new Error("Test error", exception);
        var result = Result<int>.Failure(error);
        
        result.Error.HasException.ShouldBeTrue();
        result.Error.Exception.ShouldBe(exception);
    }
}

