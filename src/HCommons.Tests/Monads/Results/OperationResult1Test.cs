using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult<>))]
public class OperationResult_1_Test {

    [Fact]
    public void Success_CreatesSuccessResult() {
        var result = OperationResult<int>.Success(42);
        result.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void Success_SetsIsFailureAndIsCancelledToFalse() {
        var result = OperationResult<int>.Success(42);
        
        result.IsFailure.ShouldBeFalse();
        result.IsCancelled.ShouldBeFalse();
    }

    [Fact]
    public void Success_StoresValue() {
        var result = OperationResult<int>.Success(42);
        result.Value.ShouldBe(42);
    }

    [Fact]
    public void Failure_CreatesFailureResult() {
        var error = new Error("Test error");
        var result = OperationResult<int>.Failure(error);
        
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void Failure_SetsIsSuccessAndIsCancelledToFalse() {
        var error = new Error("Test error");
        var result = OperationResult<int>.Failure(error);
        
        result.IsSuccess.ShouldBeFalse();
        result.IsCancelled.ShouldBeFalse();
    }

    [Fact]
    public void Failure_StoresError() {
        var error = new Error("Test error");
        var result = OperationResult<int>.Failure(error);
        
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Cancelled_CreatesCancelledResult() {
        var cancelled = new Cancelled("User cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);
        
        result.IsCancelled.ShouldBeTrue();
    }

    [Fact]
    public void Cancelled_SetsIsSuccessAndIsFailureToFalse() {
        var cancelled = new Cancelled("User cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);
        
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
    }

    [Fact]
    public void Cancelled_StoresCancellation() {
        var cancelled = new Cancelled("User cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);
        
        result.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure() {
        OperationResult<int> result = new Error("Test error");
        
        result.IsFailure.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromError_StoresError() {
        var error = new Error("Test error");
        OperationResult<int> result = error;
        
        result.Error.Message.ShouldBe("Test error");
    }

    [Fact]
    public void ImplicitConversion_FromCancelled_CreatesCancelled() {
        OperationResult<int> result = new Cancelled("User cancelled");
        
        result.IsCancelled.ShouldBeTrue();
    }

    [Fact]
    public void ImplicitConversion_FromCancelled_StoresCancellation() {
        var cancelled = new Cancelled("User cancelled");
        OperationResult<int> result = cancelled;
        
        result.Cancellation.Reason.ShouldBe("User cancelled");
    }

    [Fact]
    public void TryGetValue_WhenSuccess_ReturnsTrue() {
        var result = OperationResult<int>.Success(42);
        
        var success = result.TryGetValue(out var value);
        
        success.ShouldBeTrue();
        value.ShouldBe(42);
    }

    [Fact]
    public void TryGetValue_WhenFailure_ReturnsFalse() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        
        var success = result.TryGetValue(out _);
        
        success.ShouldBeFalse();
    }

    [Fact]
    public void TryGetValue_WhenCancelled_ReturnsFalse() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        
        var success = result.TryGetValue(out _);
        
        success.ShouldBeFalse();
    }

    [Fact]
    public void GetValueOrDefault_WhenSuccess_ReturnsValue() {
        var result = OperationResult<int>.Success(42);
        
        result.GetValueOrDefault().ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_WhenFailure_ReturnsDefault() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        
        result.GetValueOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetValueOrDefault_WhenCancelled_ReturnsDefault() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        
        result.GetValueOrDefault().ShouldBe(default(int));
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_WhenSuccess_ReturnsValue() {
        var result = OperationResult<int>.Success(42);
        
        result.GetValueOrDefault(100).ShouldBe(42);
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_WhenFailure_ReturnsDefaultValue() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        
        result.GetValueOrDefault(100).ShouldBe(100);
    }

    [Fact]
    public void GetValueOrDefault_WithDefaultValue_WhenCancelled_ReturnsDefaultValue() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        
        result.GetValueOrDefault(100).ShouldBe(100);
    }

    [Fact]
    public void Match_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int>.Success(42);
        
        var output = result.Match(
            value => value * 2,
            _ => 0,
            _ => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        
        var output = result.Match(
            value => value * 2,
            error => error.Message.Length,
            _ => 0
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public void Match_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        
        var output = result.Match(
            value => value * 2,
            _ => 0,
            cancelled => cancelled.Reason.Length
        );
        
        output.ShouldBe(14);
    }

    [Fact]
    public void Match_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int>.Success(42);
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, _) => state,
            (state, _) => state
        );
        
        output.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, error) => state + error.Message.Length,
            (state, _) => state
        );
        
        output.ShouldBe(20);
    }

    [Fact]
    public void Match_WithState_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, _) => state,
            (state, cancelled) => state + cancelled.Reason.Length
        );
        
        output.ShouldBe(24);
    }

    [Fact]
    public void Switch_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int>.Success(42);
        var successCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => { },
            _ => { }
        );
        
        successCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        var failureCalled = false;
        
        result.Switch(
            _ => { },
            _ => failureCalled = true,
            _ => { }
        );
        
        failureCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        var cancelledCalled = false;
        
        result.Switch(
            _ => { },
            _ => { },
            _ => cancelledCalled = true
        );
        
        cancelledCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult<int>.Success(42);
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state,
            (state, _) => counter = state
        );
        
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, error) => counter = state + error.Message.Length,
            (state, _) => counter = state
        );
        
        counter.ShouldBe(110);
    }

    [Fact]
    public void Switch_WithState_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state,
            (state, cancelled) => counter = state + cancelled.Reason.Length
        );
        
        counter.ShouldBe(114);
    }

    [Fact]
    public void ToString_WhenSuccess_ReturnsFormattedString() {
        var result = OperationResult<int>.Success(42);
        
        result.ToString().ShouldBe("Success: 42");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFormattedString() {
        var result = OperationResult<int>.Failure(new Error("Test error"));
        var output = result.ToString();
        
        output.ShouldStartWith("Failure:");
        output.ShouldContain("Test error");
    }

    [Fact]
    public void ToString_WhenCancelled_ReturnsFormattedString() {
        var result = OperationResult<int>.Cancelled(new Cancelled("User cancelled"));
        var output = result.ToString();
        
        output.ShouldStartWith("Cancelled:");
        output.ShouldContain("User cancelled");
    }

    [Fact]
    public void Success_WithReferenceType_StoresReference() {
        var list = new List<int> { 1, 2, 3 };
        var result = OperationResult<List<int>>.Success(list);
        
        result.Value.ShouldBeSameAs(list);
    }
}

