using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Result))]
public class ResultTest {

    [Fact]
    public void Success_CreatesSuccessResult() {
        var result = Result.Success();
        
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Success_HasEmptyError() {
        var result = Result.Success();
        
        result.Error.ShouldBe(Error.Empty);
    }

    [Fact]
    public void Failure_CreatesFailureResult() {
        var error = new Error("Test error");
        var result = Result.Failure(error);
        
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Failure_StoresError() {
        var error = new Error("Test error");
        var result = Result.Failure(error);
        
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure() {
        var error = new Error("Test error");
        Result result = error;
        
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Match_WhenSuccess_ExecutesOnSuccess() {
        var result = Result.Success();
        
        var output = result.Match(
            () => "success",
            _ => "failure"
        );
        
        output.ShouldBe("success");
    }

    [Fact]
    public void Match_WhenFailure_ExecutesOnFailure() {
        var result = Result.Failure(new Error("Test error"));
        
        var output = result.Match(
            () => "success",
            error => $"failure: {error.Message}"
        );
        
        output.ShouldBe("failure: Test error");
    }

    [Fact]
    public void Match_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result.Success();
        
        var output = result.Match(
            10,
            state => state * 2,
            (state, _) => state
        );
        
        output.ShouldBe(20);
    }

    [Fact]
    public void Match_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result.Failure(new Error("Test error"));
        
        var output = result.Match(
            10,
            state => state * 2,
            (state, error) => state + error.Message.Length
        );
        
        output.ShouldBe(20);
    }

    [Fact]
    public void Switch_WhenSuccess_ExecutesOnSuccess() {
        var result = Result.Success();
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            () => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeTrue();
        failureCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenFailure_ExecutesOnFailure() {
        var result = Result.Failure(new Error("Test error"));
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            () => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result.Success();
        var counter = 0;
        
        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1
        );
        
        counter.ShouldBe(100);
    }

    [Fact]
    public void Switch_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result.Failure(new Error("Test error"));
        var counter = 0;
        
        result.Switch(
            100,
            state => counter = state,
            (state, error) => counter = state + error.Message.Length
        );
        
        counter.ShouldBe(110);
    }

    [Fact]
    public void ToString_WhenSuccess_ReturnsSuccess() {
        var result = Result.Success();
        
        result.ToString().ShouldBe("Success");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFormattedString() {
        var result = Result.Failure(new Error("Test error"));
        
        result.ToString().ShouldStartWith("Failure:");
        result.ToString().ShouldContain("Test error");
    }

    [Fact]
    public void Failure_WithErrorWithException_PreservesException() {
        var exception = new InvalidOperationException("Operation failed");
        var error = new Error("Test error", exception);
        var result = Result.Failure(error);
        
        result.Error.HasException.ShouldBeTrue();
        result.Error.Exception.ShouldBe(exception);
    }
}

