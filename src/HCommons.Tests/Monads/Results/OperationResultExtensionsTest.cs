using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResultExtensions))]
public class OperationResultExtensionsTest {
    [Fact]
    public void MapError_OnFailure_TransformsError() {
        var error = new Error("original error");
        var result = OperationResult.Failure(error);

        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public void MapError_OnSuccess_ReturnsOriginal() {
        var result = OperationResult.Success();

        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapError_OnCancelled_ReturnsOriginal() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);

        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void MapError_WithState_OnFailure_TransformsError() {
        var error = new Error("original error");
        var result = OperationResult.Failure(error);
        var state = 42;

        var actual = result.MapError(state, (s, e) => new Error($"transformed: {e.Message}, state: {s}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error, state: 42");
    }

    [Fact]
    public void MapCancellation_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var result = OperationResult.Cancelled(cancelled);

        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public void MapCancellation_OnSuccess_ReturnsOriginal() {
        var result = OperationResult.Success();

        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapCancellation_OnFailure_ReturnsOriginal() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);

        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public void MapCancellation_WithState_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var result = OperationResult.Cancelled(cancelled);
        var state = 42;

        var actual = result.MapCancellation(state, (s, c) => new Cancelled($"transformed: {c.Reason}, state: {s}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason, state: 42");
    }

    [Fact]
    public void Match_OnSuccess_ExecutesSuccessFunction() {
        var result = OperationResult.Success();

        var actual = result.Match(() => "success", _ => "failure", _ => "cancelled");

        actual.ShouldBe("success");
    }

    [Fact]
    public void Match_OnFailure_ExecutesFailureFunction() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);

        var actual = result.Match(() => "success", e => $"failure: {e.Message}", _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public void Match_OnCancelled_ExecutesCancelledFunction() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);

        var actual = result.Match(() => "success", _ => "failure", c => $"cancelled: {c.Reason}");

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public void Match_WithState_OnSuccess_ExecutesSuccessFunctionWithState() {
        var result = OperationResult.Success();
        var state = "my state";

        var actual = result.Match(
            state,
            s => $"success: {s}",
            (s, _) => $"failure: {s}",
            (s, _) => $"cancelled: {s}"
        );

        actual.ShouldBe("success: my state");
    }

    [Fact]
    public void Match_WithState_OnFailure_ExecutesFailureFunctionWithState() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);
        var state = "my state";

        var actual = result.Match(
            state,
            s => $"success: {s}",
            (s, e) => $"failure: {s}, {e.Message}",
            (s, _) => $"cancelled: {s}"
        );

        actual.ShouldBe("failure: my state, test error");
    }

    [Fact]
    public void Match_WithState_OnCancelled_ExecutesCancelledFunctionWithState() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);
        var state = "my state";

        var actual = result.Match(
            state,
            s => $"success: {s}",
            (s, _) => $"failure: {s}",
            (s, c) => $"cancelled: {s}, {c.Reason}"
        );

        actual.ShouldBe("cancelled: my state, user cancelled");
    }

    [Fact]
    public void Switch_OnSuccess_ExecutesSuccessAction() {
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
    public void Switch_OnFailure_ExecutesFailureAction() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);
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
    public void Switch_OnCancelled_ExecutesCancelledAction() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);
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
    public void Switch_WithState_OnSuccess_ExecutesSuccessActionWithState() {
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
    public void Switch_WithState_OnFailure_ExecutesFailureActionWithState() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);
        var counter = 0;

        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        counter.ShouldBe(101);
    }

    [Fact]
    public void Switch_WithState_OnCancelled_ExecutesCancelledActionWithState() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);
        var counter = 0;

        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        counter.ShouldBe(102);
    }
}
