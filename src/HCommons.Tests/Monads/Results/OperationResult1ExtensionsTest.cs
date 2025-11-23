using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult1Extensions))]
public class OperationResult1ExtensionsTest {
    [Fact]
    public void Select_OnSuccess_TransformsValue() {
        var result = OperationResult<int>.Success(10);

        var actual = result.Select(x => x * 2);

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(20);
    }

    [Fact]
    public void Select_OnFailure_ReturnsFailure() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = result.Select(x => x * 2);

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public void Select_OnCancelled_ReturnsCancelled() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = result.Select(x => x * 2);

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void Select_WithState_OnSuccess_TransformsValueWithState() {
        var result = OperationResult<int>.Success(10);
        var state = 5;

        var actual = result.Select(state, (s, x) => x * s);

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(50);
    }

    [Fact]
    public void Bind_OnSuccess_ReturnsBinderResult() {
        var result = OperationResult<int>.Success(10);

        var actual = result.Bind(x => OperationResult<string>.Success(x.ToString()));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("10");
    }

    [Fact]
    public void Bind_OnFailure_ReturnsFailure() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = result.Bind(x => OperationResult<string>.Success(x.ToString()));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public void Bind_OnCancelled_ReturnsCancelled() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = result.Bind(x => OperationResult<string>.Success(x.ToString()));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void Bind_WithState_OnSuccess_ReturnsBinderResultWithState() {
        var result = OperationResult<int>.Success(10);
        var state = "prefix";

        var actual = result.Bind(state, (s, x) => OperationResult<string>.Success($"{s}{x}"));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("prefix10");
    }

    [Fact]
    public void MapError_OnFailure_TransformsError() {
        var error = new Error("original error");
        var result = OperationResult<int>.Failure(error);

        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public void MapError_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int>.Success(10);

        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(10);
    }

    [Fact]
    public void MapError_OnCancelled_ReturnsOriginal() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void MapError_WithState_OnFailure_TransformsError() {
        var error = new Error("original error");
        var result = OperationResult<int>.Failure(error);
        var state = 42;

        var actual = result.MapError(state, (s, e) => new Error($"transformed: {e.Message}, state: {s}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error, state: 42");
    }

    [Fact]
    public void MapCancellation_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public void MapCancellation_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int>.Success(10);

        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(10);
    }

    [Fact]
    public void MapCancellation_OnFailure_ReturnsOriginal() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public void MapCancellation_WithState_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var result = OperationResult<int>.Cancelled(cancelled);
        var state = 42;

        var actual = result.MapCancellation(state, (s, c) => new Cancelled($"transformed: {c.Reason}, state: {s}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason, state: 42");
    }

    [Fact]
    public void Match_OnSuccess_ExecutesSuccessFunction() {
        var result = OperationResult<int>.Success(10);

        var actual = result.Match(
            x => $"success: {x}",
            _ => "failure",
            _ => "cancelled"
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public void Match_OnFailure_ExecutesFailureFunction() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = result.Match(
            x => $"success: {x}",
            e => $"failure: {e.Message}",
            _ => "cancelled"
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public void Match_OnCancelled_ExecutesCancelledFunction() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = result.Match(
            x => $"success: {x}",
            _ => "failure",
            c => $"cancelled: {c.Reason}"
        );

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public void Match_WithState_OnSuccess_ExecutesSuccessFunctionWithState() {
        var result = OperationResult<int>.Success(10);
        var state = "my state";

        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, _) => $"failure: {s}",
            (s, _) => $"cancelled: {s}"
        );

        actual.ShouldBe("success: my state, 10");
    }

    [Fact]
    public void Match_WithState_OnFailure_ExecutesFailureFunctionWithState() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);
        var state = "my state";

        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, e) => $"failure: {s}, {e.Message}",
            (s, _) => $"cancelled: {s}"
        );

        actual.ShouldBe("failure: my state, test error");
    }

    [Fact]
    public void Match_WithState_OnCancelled_ExecutesCancelledFunctionWithState() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);
        var state = "my state";

        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, _) => $"failure: {s}",
            (s, c) => $"cancelled: {s}, {c.Reason}"
        );

        actual.ShouldBe("cancelled: my state, user cancelled");
    }

    [Fact]
    public void Switch_OnSuccess_ExecutesSuccessAction() {
        var result = OperationResult<int>.Success(10);
        var successValue = 0;
        var failureCalled = false;
        var cancelledCalled = false;

        result.Switch(
            x => successValue = x,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );

        successValue.ShouldBe(10);
        failureCalled.ShouldBeFalse();
        cancelledCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_OnFailure_ExecutesFailureAction() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;

        result.Switch(
            _ => successCalled = true,
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
        var result = OperationResult<int>.Cancelled(cancelled);
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;

        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );

        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeFalse();
        cancelledCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_OnSuccess_ExecutesSuccessActionWithState() {
        var result = OperationResult<int>.Success(10);
        var counter = 0;

        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        counter.ShouldBe(110);
    }

    [Fact]
    public void Switch_WithState_OnFailure_ExecutesFailureActionWithState() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);
        var counter = 0;

        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        counter.ShouldBe(101);
    }

    [Fact]
    public void Switch_WithState_OnCancelled_ExecutesCancelledActionWithState() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);
        var counter = 0;

        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        counter.ShouldBe(102);
    }
}
