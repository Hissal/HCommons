using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Result<>))]
public class Result_1_Test {

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
    public void Match_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        
        var output = result.Match(
            value => value * 2,
            _ => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var output = result.Match(
            value => value * 2,
            error => error.Message.Length
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public void Match_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, _) => state
        );
        
        output.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, error) => state + error.Message.Length
        );
        
        output.ShouldBe(20);
    }

    [Fact]
    public void Switch_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeTrue();
        failureCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state
        );
        
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, error) => counter = state + error.Message.Length
        );
        
        counter.ShouldBe(110);
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

