using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult3Extensions))]
public class OperationResult3ExtensionsTest {
    [Fact]
    public void Select_OnSuccess_TransformsSuccessValue() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = result.Select(x => x * 2);

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(20);
    }

    [Fact]
    public void Select_OnFailure_ReturnsFailure() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = result.Select(x => x * 2);

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void Select_OnCancelled_ReturnsCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = result.Select(x => x * 2);

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void Select_WithState_OnSuccess_TransformsSuccessValueWithState() {
        var result = OperationResult<int, string, bool>.Success(10);
        var state = 5;

        var actual = result.Select(state, (s, x) => x * s);

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(50);
    }

    [Fact]
    public void Select_WithState_OnFailure_ReturnsFailure() {
        var result = OperationResult<int, string, bool>.Failure("test error");
        var state = 5;

        var actual = result.Select(state, (s, x) => x * s);

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void Select_WithState_OnCancelled_ReturnsCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var state = 5;

        var actual = result.Select(state, (s, x) => x * s);

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void Bind_OnSuccess_ReturnsBinderResult() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = result.Bind(x => OperationResult<double, string, bool>.Success(x / 2.0));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(5.0);
    }

    [Fact]
    public void Bind_OnFailure_ReturnsFailure() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = result.Bind(x => OperationResult<double, string, bool>.Success(x / 2.0));

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void Bind_OnCancelled_ReturnsCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = result.Bind(x => OperationResult<double, string, bool>.Success(x / 2.0));

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void Bind_WithState_OnSuccess_ReturnsBinderResultWithState() {
        var result = OperationResult<int, string, bool>.Success(10);
        var state = 2;

        var actual = result.Bind(state, (s, x) => OperationResult<double, string, bool>.Success(((double)x * s) / 2.0));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10.0);
    }

    [Fact]
    public void Bind_WithState_OnFailure_ReturnsFailure() {
        var result = OperationResult<int, string, bool>.Failure("test error");
        var state = 2;

        var actual = result.Bind(state, (s, x) => OperationResult<double, string, bool>.Success(((double)x * s) / 2.0));

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void Bind_WithState_OnCancelled_ReturnsCancelled() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var state = 2;

        var actual = result.Bind(state, (s, x) => OperationResult<double, string, bool>.Success(((double)x * s) / 2.0));

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void MapError_OnFailure_TransformsFailureValue() {
        var result = OperationResult<int, string, bool>.Failure("original error");

        var actual = result.MapError(f => $"transformed: {f}");

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("transformed: original error");
    }

    [Fact]
    public void MapError_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = result.MapError(f => $"transformed: {f}");

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public void MapError_OnCancelled_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = result.MapError(f => $"transformed: {f}");

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void MapError_WithState_OnFailure_TransformsFailureValue() {
        var result = OperationResult<int, string, bool>.Failure("original error");
        var state = 42;

        var actual = result.MapError(state, (s, f) => $"transformed: {f}, state: {s}");

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("transformed: original error, state: 42");
    }

    [Fact]
    public void MapError_WithState_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Success(10);
        var state = 42;

        var actual = result.MapError(state, (s, f) => $"transformed: {f}, state: {s}");

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public void MapError_WithState_OnCancelled_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var state = 42;

        var actual = result.MapError(state, (s, f) => $"transformed: {f}, state: {s}");

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void MapCancellation_OnCancelled_TransformsCancellationValue() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = result.MapCancellation(c => !c);

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(false);
    }

    [Fact]
    public void MapCancellation_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = result.MapCancellation(c => !c);

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public void MapCancellation_OnFailure_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = result.MapCancellation(c => !c);

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void MapCancellation_WithState_OnCancelled_TransformsCancellationValue() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var state = "prefix";

        var actual = result.MapCancellation(state, (s, c) => $"{s}{c}");

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe("prefixTrue");
    }

    [Fact]
    public void MapCancellation_WithState_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Success(10);
        var state = "prefix";

        var actual = result.MapCancellation(state, (s, c) => $"{s}{c}");

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public void MapCancellation_WithState_OnFailure_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Failure("test error");
        var state = "prefix";

        var actual = result.MapCancellation(state, (s, c) => $"{s}{c}");

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void Match_OnSuccess_ExecutesSuccessFunction() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = result.Match(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public void Match_OnFailure_ExecutesFailureFunction() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = result.Match(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public void Match_OnCancelled_ExecutesCancelledFunction() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = result.Match(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("cancelled: True");
    }

    [Fact]
    public void Match_WithState_OnSuccess_ExecutesSuccessFunctionWithState() {
        var result = OperationResult<int, string, bool>.Success(10);
        var state = "my state";

        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, f) => $"failure: {s}, {f}",
            (s, c) => $"cancelled: {s}, {c}"
        );

        actual.ShouldBe("success: my state, 10");
    }

    [Fact]
    public void Match_WithState_OnFailure_ExecutesFailureFunctionWithState() {
        var result = OperationResult<int, string, bool>.Failure("test error");
        var state = "my state";

        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, f) => $"failure: {s}, {f}",
            (s, c) => $"cancelled: {s}, {c}"
        );

        actual.ShouldBe("failure: my state, test error");
    }

    [Fact]
    public void Match_WithState_OnCancelled_ExecutesCancelledFunctionWithState() {
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var state = "my state";

        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, f) => $"failure: {s}, {f}",
            (s, c) => $"cancelled: {s}, {c}"
        );

        actual.ShouldBe("cancelled: my state, True");
    }

    [Fact]
    public void Switch_OnSuccess_ExecutesSuccessAction() {
        var result = OperationResult<int, string, bool>.Success(10);
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
        var result = OperationResult<int, string, bool>.Failure("test error");
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
        var result = OperationResult<int, string, bool>.Cancelled(true);
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
        var result = OperationResult<int, string, bool>.Success(10);
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
        var result = OperationResult<int, string, bool>.Failure("test error");
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
        var result = OperationResult<int, string, bool>.Cancelled(true);
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
