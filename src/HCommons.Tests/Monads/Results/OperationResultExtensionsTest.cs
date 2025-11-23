using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResultExtensions))]
public class OperationResultExtensionsTest {
    [Fact]
    public void MapError_OnFailure_TransformsError() {
        // Arrange
        var error = new Error("original error");
        var result = OperationResult.Failure(error);

        // Act
        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public void MapError_OnSuccess_ReturnsOriginal() {
        // Arrange
        var result = OperationResult.Success();

        // Act
        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapError_OnCancelled_ReturnsOriginal() {
        // Arrange
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);

        // Act
        var actual = result.MapError(e => new Error($"transformed: {e.Message}"));

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public void MapError_WithState_OnFailure_TransformsError() {
        // Arrange
        var error = new Error("original error");
        var result = OperationResult.Failure(error);
        var state = 42;

        // Act
        var actual = result.MapError(state, (s, e) => new Error($"transformed: {e.Message}, state: {s}"));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error, state: 42");
    }

    [Fact]
    public void MapCancellation_OnCancelled_TransformsCancellation() {
        // Arrange
        var cancelled = new Cancelled("original reason");
        var result = OperationResult.Cancelled(cancelled);

        // Act
        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public void MapCancellation_OnSuccess_ReturnsOriginal() {
        // Arrange
        var result = OperationResult.Success();

        // Act
        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapCancellation_OnFailure_ReturnsOriginal() {
        // Arrange
        var error = new Error("test error");
        var result = OperationResult.Failure(error);

        // Act
        var actual = result.MapCancellation(c => new Cancelled($"transformed: {c.Reason}"));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public void MapCancellation_WithState_OnCancelled_TransformsCancellation() {
        // Arrange
        var cancelled = new Cancelled("original reason");
        var result = OperationResult.Cancelled(cancelled);
        var state = 42;

        // Act
        var actual = result.MapCancellation(state, (s, c) => new Cancelled($"transformed: {c.Reason}, state: {s}"));

        // Assert
        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason, state: 42");
    }

    [Fact]
    public void Match_OnSuccess_ExecutesSuccessFunction() {
        // Arrange
        var result = OperationResult.Success();

        // Act
        var actual = result.Match(() => "success", _ => "failure", _ => "cancelled");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public void Match_OnFailure_ExecutesFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var result = OperationResult.Failure(error);

        // Act
        var actual = result.Match(() => "success", e => $"failure: {e.Message}", _ => "cancelled");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public void Match_OnCancelled_ExecutesCancelledFunction() {
        // Arrange
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);

        // Act
        var actual = result.Match(() => "success", _ => "failure", c => $"cancelled: {c.Reason}");

        // Assert
        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public void Match_WithState_OnSuccess_ExecutesSuccessFunctionWithState() {
        // Arrange
        var result = OperationResult.Success();
        var state = "my state";

        // Act
        var actual = result.Match(
            state,
            s => $"success: {s}",
            (s, _) => $"failure: {s}",
            (s, _) => $"cancelled: {s}"
        );

        // Assert
        actual.ShouldBe("success: my state");
    }

    [Fact]
    public void Match_WithState_OnFailure_ExecutesFailureFunctionWithState() {
        // Arrange
        var error = new Error("test error");
        var result = OperationResult.Failure(error);
        var state = "my state";

        // Act
        var actual = result.Match(
            state,
            s => $"success: {s}",
            (s, e) => $"failure: {s}, {e.Message}",
            (s, _) => $"cancelled: {s}"
        );

        // Assert
        actual.ShouldBe("failure: my state, test error");
    }

    [Fact]
    public void Match_WithState_OnCancelled_ExecutesCancelledFunctionWithState() {
        // Arrange
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);
        var state = "my state";

        // Act
        var actual = result.Match(
            state,
            s => $"success: {s}",
            (s, _) => $"failure: {s}",
            (s, c) => $"cancelled: {s}, {c.Reason}"
        );

        // Assert
        actual.ShouldBe("cancelled: my state, user cancelled");
    }

    [Fact]
    public void Switch_OnSuccess_ExecutesSuccessAction() {
        // Arrange
        var result = OperationResult.Success();
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;

        // Act
        result.Switch(
            () => successCalled = true,
            _ => failureCalled = true,
            _ => cancelledCalled = true
        );

        // Assert
        successCalled.ShouldBeTrue();
        failureCalled.ShouldBeFalse();
        cancelledCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_OnFailure_ExecutesFailureAction() {
        // Arrange
        var error = new Error("test error");
        var result = OperationResult.Failure(error);
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;

        // Act
        result.Switch(
            () => successCalled = true,
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
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);
        var successCalled = false;
        var failureCalled = false;
        var cancelledCalled = false;

        // Act
        result.Switch(
            () => successCalled = true,
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
        var result = OperationResult.Success();
        var counter = 0;

        // Act
        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        // Assert
        counter.ShouldBe(100);
    }

    [Fact]
    public void Switch_WithState_OnFailure_ExecutesFailureActionWithState() {
        // Arrange
        var error = new Error("test error");
        var result = OperationResult.Failure(error);
        var counter = 0;

        // Act
        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        // Assert
        counter.ShouldBe(101);
    }

    [Fact]
    public void Switch_WithState_OnCancelled_ExecutesCancelledActionWithState() {
        // Arrange
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);
        var counter = 0;

        // Act
        result.Switch(
            100,
            state => counter = state,
            (state, _) => counter = state + 1,
            (state, _) => counter = state + 2
        );

        // Assert
        counter.ShouldBe(102);
    }
}
