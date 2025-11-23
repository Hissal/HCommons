using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResultExtensionsAsync))]
public class OperationResultExtensionsAsyncTest {
    [Fact]
    public async Task MapErrorAsync_TaskResult_OnFailure_TransformsError() {
        var error = new Error("original error");
        var resultTask = Task.FromResult(OperationResult.Failure(error));

        var actual = await resultTask.MapErrorAsync(e => new Error($"transformed: {e.Message}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnSuccess_ReturnsOriginal() {
        var resultTask = Task.FromResult(OperationResult.Success());

        var actual = await resultTask.MapErrorAsync(e => new Error($"transformed: {e.Message}"));

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnCancelled_ReturnsOriginal() {
        var cancelled = new Cancelled("user cancelled");
        var resultTask = Task.FromResult(OperationResult.Cancelled(cancelled));

        var actual = await resultTask.MapErrorAsync(e => new Error($"transformed: {e.Message}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnFailure_TransformsError() {
        var error = new Error("original error");
        var result = OperationResult.Failure(error);

        var actual = await result.MapErrorAsync(e => Task.FromResult(new Error($"transformed: {e.Message}")));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnSuccess_ReturnsOriginal() {
        var result = OperationResult.Success();

        var actual = await result.MapErrorAsync(e => Task.FromResult(new Error($"transformed: {e.Message}")));

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task MapErrorAsync_TaskResultAsyncMapper_OnFailure_TransformsError() {
        var error = new Error("original error");
        var resultTask = Task.FromResult(OperationResult.Failure(error));

        var actual = await resultTask.MapErrorAsync(e => Task.FromResult(new Error($"transformed: {e.Message}")));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResult_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var resultTask = Task.FromResult(OperationResult.Cancelled(cancelled));

        var actual = await resultTask.MapCancellationAsync(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResult_OnSuccess_ReturnsOriginal() {
        var resultTask = Task.FromResult(OperationResult.Success());

        var actual = await resultTask.MapCancellationAsync(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResult_OnFailure_ReturnsOriginal() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult.Failure(error));

        var actual = await resultTask.MapCancellationAsync(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public async Task MapCancellationAsync_AsyncMapper_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var result = OperationResult.Cancelled(cancelled);

        var actual = await result.MapCancellationAsync(c => Task.FromResult(new Cancelled($"transformed: {c.Reason}")));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public async Task MapCancellationAsync_AsyncMapper_OnSuccess_ReturnsOriginal() {
        var result = OperationResult.Success();

        var actual = await result.MapCancellationAsync(c => Task.FromResult(new Cancelled($"transformed: {c.Reason}")));

        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResultAsyncMapper_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var resultTask = Task.FromResult(OperationResult.Cancelled(cancelled));

        var actual = await resultTask.MapCancellationAsync(c => Task.FromResult(new Cancelled($"transformed: {c.Reason}")));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult.Success());

        var actual = await resultTask.MatchAsync(() => "success", _ => "failure", _ => "cancelled");

        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult.Failure(error));

        var actual = await resultTask.MatchAsync(() => "success", e => $"failure: {e.Message}", _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var resultTask = Task.FromResult(OperationResult.Cancelled(cancelled));

        var actual = await resultTask.MatchAsync(() => "success", _ => "failure", c => $"cancelled: {c.Reason}");

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnSuccess_ExecutesSuccessHandler() {
        var result = OperationResult.Success();

        var actual = await result.MatchAsync(() => Task.FromResult("success"), _ => "failure", _ => "cancelled");

        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnFailure_DoesNotCallAsyncSuccess() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);

        var actual = await result.MatchAsync(() => Task.FromResult("success"), e => $"failure: {e.Message}", _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);

        var actual = await result.MatchAsync(() => "success", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnSuccess_DoesNotCallAsyncFailure() {
        var result = OperationResult.Success();

        var actual = await result.MatchAsync(() => "success", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AsyncCancelled_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);

        var actual = await result.MatchAsync(() => "success", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_AsyncCancelled_OnSuccess_DoesNotCallAsyncCancelled() {
        var result = OperationResult.Success();

        var actual = await result.MatchAsync(() => "success", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnSuccess_ExecutesSuccessHandler() {
        var result = OperationResult.Success();

        var actual = await result.MatchAsync(
            () => Task.FromResult("success"),
            _ => Task.FromResult("failure"),
            _ => Task.FromResult("cancelled")
        );

        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var result = OperationResult.Failure(error);

        var actual = await result.MatchAsync(
            () => Task.FromResult("success"),
            e => Task.FromResult($"failure: {e.Message}"),
            _ => Task.FromResult("cancelled")
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);

        var actual = await result.MatchAsync(
            () => Task.FromResult("success"),
            _ => Task.FromResult("failure"),
            c => Task.FromResult($"cancelled: {c.Reason}")
        );

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnSuccess_ExecutesSuccessHandler() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult.Success());

        // Act
        var actual = await resultTask.MatchAsync(() => Task.FromResult("success"), _ => "failure", _ => "cancelled");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnFailure_DoesNotCallAsyncSuccess() {
        // Arrange
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult.Failure(error));

        // Act
        var actual = await resultTask.MatchAsync(() => Task.FromResult("success"), e => $"failure: {e.Message}", _ => "cancelled");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnFailure_ExecutesFailureHandler() {
        // Arrange
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult.Failure(error));

        // Act
        var actual = await resultTask.MatchAsync(() => "success", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnSuccess_DoesNotCallAsyncFailure() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult.Success());

        // Act
        var actual = await resultTask.MatchAsync(() => "success", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncCancelled_OnCancelled_ExecutesCancelledHandler() {
        // Arrange
        var cancelled = new Cancelled("user cancelled");
        var resultTask = Task.FromResult(OperationResult.Cancelled(cancelled));

        // Act
        var actual = await resultTask.MatchAsync(() => "success", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        // Assert
        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncCancelled_OnSuccess_DoesNotCallAsyncCancelled() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult.Success());

        // Act
        var actual = await resultTask.MatchAsync(() => "success", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccessAndCancelled_OnSuccess_ExecutesSuccessHandler() {
        // Arrange
        var result = OperationResult.Success();

        // Act
        var actual = await result.MatchAsync(() => Task.FromResult("success"), _ => "failure", _ => Task.FromResult("cancelled"));

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccessAndCancelled_OnCancelled_ExecutesCancelledHandler() {
        // Arrange
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult.Cancelled(cancelled);

        // Act
        var actual = await result.MatchAsync(() => Task.FromResult("success"), _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        // Assert
        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccessAndFailure_OnSuccess_ExecutesSuccessHandler() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult.Success());

        // Act
        var actual = await resultTask.MatchAsync(() => Task.FromResult("success"), _ => Task.FromResult("failure"), _ => "cancelled");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccessAndFailure_OnFailure_ExecutesFailureHandler() {
        // Arrange
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult.Failure(error));

        // Act
        var actual = await resultTask.MatchAsync(() => Task.FromResult("success"), e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAllAsync_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult.Success());

        var actual = await resultTask.MatchAsync(
            () => Task.FromResult("success"),
            _ => Task.FromResult("failure"),
            _ => Task.FromResult("cancelled")
        );

        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_ChainedOperations_WorksCorrectly() {
        var error = new Error("original");
        var result = OperationResult.Failure(error);

        var actual = await result
            .MapErrorAsync(e => Task.FromResult(new Error($"{e.Message}1")))
            .MapErrorAsync(e => new Error($"{e.Message}2"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("original12");
    }
}

