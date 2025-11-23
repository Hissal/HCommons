using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult3Extensions))]
public class OperationResult3ExtensionsTest {
    [Fact]
    public void Select_OnSuccess_TransformsSuccessValue() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);

        // Act
        var actual = result.Select(x => x * 2);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(20);
    }

    [Fact]
    public void Select_OnFailure_ReturnsFailure() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("test error");

        // Act
        var actual = result.Select(x => x * 2);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void Select_OnCancelled_ReturnsCancelled() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);

        // Act
        var actual = result.Select(x => x * 2);

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void Select_WithState_OnSuccess_TransformsSuccessValueWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);
        var state = 5;

        // Act
        var actual = result.Select(state, (s, x) => x * s);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(50);
    }

    [Fact]
    public void Bind_OnSuccess_ReturnsBinderResult() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);

        // Act
        var actual = result.Bind(x => OperationResult<double, string, bool>.Success(x / 2.0));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(5.0);
    }

    [Fact]
    public void Bind_OnFailure_ReturnsFailure() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("test error");

        // Act
        var actual = result.Bind(x => OperationResult<double, string, bool>.Success(x / 2.0));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void Bind_OnCancelled_ReturnsCancelled() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);

        // Act
        var actual = result.Bind(x => OperationResult<double, string, bool>.Success(x / 2.0));

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void Bind_WithState_OnSuccess_ReturnsBinderResultWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);
        var state = 2;

        // Act
        var actual = result.Bind(state, (s, x) => OperationResult<double, string, bool>.Success((x * s) / 2.0));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10.0);
    }

    [Fact]
    public void MapError_OnFailure_TransformsFailureValue() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("original error");

        // Act
        var actual = result.MapError(f => $"transformed: {f}");

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("transformed: original error");
    }

    [Fact]
    public void MapError_OnSuccess_ReturnsOriginal() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);

        // Act
        var actual = result.MapError(f => $"transformed: {f}");

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public void MapError_OnCancelled_ReturnsOriginal() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);

        // Act
        var actual = result.MapError(f => $"transformed: {f}");

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public void MapError_WithState_OnFailure_TransformsFailureValue() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("original error");
        var state = 42;

        // Act
        var actual = result.MapError(state, (s, f) => $"transformed: {f}, state: {s}");

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("transformed: original error, state: 42");
    }

    [Fact]
    public void MapCancellation_OnCancelled_TransformsCancellationValue() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);

        // Act
        var actual = result.MapCancellation(c => !c);

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(false);
    }

    [Fact]
    public void MapCancellation_OnSuccess_ReturnsOriginal() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);

        // Act
        var actual = result.MapCancellation(c => !c);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public void MapCancellation_OnFailure_ReturnsOriginal() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("test error");

        // Act
        var actual = result.MapCancellation(c => !c);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public void MapCancellation_WithState_OnCancelled_TransformsCancellationValue() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var state = "prefix";

        // Act
        var actual = result.MapCancellation(state, (s, c) => $"{s}{c}");

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe("prefixTrue");
    }

    [Fact]
    public void Match_OnSuccess_ExecutesSuccessFunction() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);

        // Act
        var actual = result.Match(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        // Assert
        actual.ShouldBe("success: 10");
    }

    [Fact]
    public void Match_OnFailure_ExecutesFailureFunction() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("test error");

        // Act
        var actual = result.Match(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public void Match_OnCancelled_ExecutesCancelledFunction() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);

        // Act
        var actual = result.Match(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        // Assert
        actual.ShouldBe("cancelled: True");
    }

    [Fact]
    public void Match_WithState_OnSuccess_ExecutesSuccessFunctionWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);
        var state = "my state";

        // Act
        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, f) => $"failure: {s}, {f}",
            (s, c) => $"cancelled: {s}, {c}"
        );

        // Assert
        actual.ShouldBe("success: my state, 10");
    }

    [Fact]
    public void Match_WithState_OnFailure_ExecutesFailureFunctionWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("test error");
        var state = "my state";

        // Act
        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, f) => $"failure: {s}, {f}",
            (s, c) => $"cancelled: {s}, {c}"
        );

        // Assert
        actual.ShouldBe("failure: my state, test error");
    }

    [Fact]
    public void Match_WithState_OnCancelled_ExecutesCancelledFunctionWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var state = "my state";

        // Act
        var actual = result.Match(
            state,
            (s, x) => $"success: {s}, {x}",
            (s, f) => $"failure: {s}, {f}",
            (s, c) => $"cancelled: {s}, {c}"
        );

        // Assert
        actual.ShouldBe("cancelled: my state, True");
    }

    [Fact]
    public void Switch_OnSuccess_ExecutesSuccessAction() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);
        var successValue = 0;
        var failureCalled = false;
        var cancelledCalled = false;

        // Act
        result.Switch(
            x => successValue = x,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );

        // Assert
        successValue.ShouldBe(10);
        failureCalled.ShouldBeFalse();
        cancelledCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_OnFailure_ExecutesFailureAction() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("test error");
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;

        // Act
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );

        // Assert
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeTrue();
        cancelledCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_OnCancelled_ExecutesCancelledAction() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;

        // Act
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );

        // Assert
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeFalse();
        cancelledCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_OnSuccess_ExecutesSuccessActionWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);
        var counter = 0;

        // Act
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        // Assert
        counter.ShouldBe(110);
    }

    [Fact]
    public void Switch_WithState_OnFailure_ExecutesFailureActionWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Failure("test error");
        var counter = 0;

        // Act
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        // Assert
        counter.ShouldBe(101);
    }

    [Fact]
    public void Switch_WithState_OnCancelled_ExecutesCancelledActionWithState() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);
        var counter = 0;

        // Act
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        // Assert
        counter.ShouldBe(102);
    }
}
