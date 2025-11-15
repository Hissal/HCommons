using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult))]
public class OperationResultTest {

    [Fact]
    public void Success_CreatesSuccessResult() {
        var result = OperationResult.Success();
        
        result.IsSuccess.ShouldBeTrue();
        result.IsFailure.ShouldBeFalse();
        result.IsCancelled.ShouldBeFalse();
    }

    [Fact]
    public void Success_HasEmptyErrorAndCancellation() {
        var result = OperationResult.Success();
        
        result.Error.ShouldBe(Error.Empty);
        result.Cancellation.ShouldBe(Cancelled.Empty);
    }

    [Fact]
    public void Failure_CreatesFailureResult() {
        var error = new Error("Test error");
        var result = OperationResult.Failure(error);
        
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeTrue();
        result.IsCancelled.ShouldBeFalse();
    }

    [Fact]
    public void Failure_StoresError() {
        var error = new Error("Test error");
        var result = OperationResult.Failure(error);
        
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void Cancelled_CreatesCancelledResult() {
        var cancelled = new Cancelled("User cancelled");
        var result = OperationResult.Cancelled(cancelled);
        
        result.IsSuccess.ShouldBeFalse();
        result.IsFailure.ShouldBeFalse();
        result.IsCancelled.ShouldBeTrue();
    }

    [Fact]
    public void Cancelled_StoresCancellation() {
        var cancelled = new Cancelled("User cancelled");
        var result = OperationResult.Cancelled(cancelled);
        
        result.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void ImplicitConversion_FromError_CreatesFailure() {
        var error = new Error("Test error");
        OperationResult result = error;
        
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(error);
    }

    [Fact]
    public void ImplicitConversion_FromCancelled_CreatesCancelled() {
        var cancelled = new Cancelled("User cancelled");
        OperationResult result = cancelled;
        
        result.IsCancelled.ShouldBeTrue();
        result.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void Match_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult.Success();
        
        var output = result.Match(
            () => "success",
            _ => "failure",
            _ => "cancelled"
        );
        
        output.ShouldBe("success");
    }

    [Fact]
    public void Match_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult.Failure(new Error("Test error"));
        
        var output = result.Match(
            () => "success",
            error => $"failure: {error.Message}",
            _ => "cancelled"
        );
        
        output.ShouldBe("failure: Test error");
    }

    [Fact]
    public void Match_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult.Cancelled(new Cancelled("User cancelled"));
        
        var output = result.Match(
            () => "success",
            _ => "failure",
            cancelled => $"cancelled: {cancelled.Reason}"
        );
        
        output.ShouldBe("cancelled: User cancelled");
    }

    [Fact]
    public void Match_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult.Success();
        
        var output = result.Match(
            10,
            state => state * 2,
            (state, _) => state,
            (state, _) => state
        );
        
        output.ShouldBe(20);
    }

    [Fact]
    public void Match_WithState_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult.Failure(new Error("Test error"));
        
        var output = result.Match(
            10,
            state => state * 2,
            (state, error) => state + error.Message.Length,
            (state, _) => state
        );
        
        output.ShouldBe(20);
    }

    [Fact]
    public void Match_WithState_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult.Cancelled(new Cancelled("User cancelled"));
        
        var output = result.Match(
            10,
            state => state * 2,
            (state, _) => state,
            (state, cancelled) => state + cancelled.Reason.Length
        );
        
        output.ShouldBe(24);
    }

    [Fact]
    public void Switch_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult.Success();
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;
        
        result.Switch(
            () => successCalled = true,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );
        
        successCalled.ShouldBeTrue();
        failureCalled.ShouldBeFalse();
        cancelledCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult.Failure(new Error("Test error"));
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;
        
        result.Switch(
            () => successCalled = true,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );
        
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeTrue();
        cancelledCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult.Cancelled(new Cancelled("User cancelled"));
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;
        
        result.Switch(
            () => successCalled = true,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );
        
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeFalse();
        cancelledCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = OperationResult.Success();
        var counter = 0;
        
        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );
        
        counter.ShouldBe(100);
    }

    [Fact]
    public void Switch_WithState_WhenFailure_ExecutesOnFailure() {
        var result = OperationResult.Failure(new Error("Test error"));
        var counter = 0;
        
        result.Switch(
            100,
            state => counter = state,
            (state, error) => counter = state + error.Message.Length,
            (state, _) => counter = state + 2
        );
        
        counter.ShouldBe(110);
    }

    [Fact]
    public void Switch_WithState_WhenCancelled_ExecutesOnCancelled() {
        var result = OperationResult.Cancelled(new Cancelled("User cancelled"));
        var counter = 0;
        
        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1,
            (state, cancelled) => counter = state + cancelled.Reason.Length
        );
        
        counter.ShouldBe(114);
    }

    [Fact]
    public void ToString_WhenSuccess_ReturnsSuccess() {
        var result = OperationResult.Success();
        
        result.ToString().ShouldBe("Success");
    }

    [Fact]
    public void ToString_WhenFailure_ReturnsFormattedString() {
        var result = OperationResult.Failure(new Error("Test error"));
        
        result.ToString().ShouldStartWith("Failure:");
        result.ToString().ShouldContain("Test error");
    }

    [Fact]
    public void ToString_WhenCancelled_ReturnsFormattedString() {
        var result = OperationResult.Cancelled(new Cancelled("User cancelled"));
        
        result.ToString().ShouldStartWith("Cancelled:");
        result.ToString().ShouldContain("User cancelled");
    }
}

