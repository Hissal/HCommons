using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult1ExtensionsAsync))]
public class OperationResult1ExtensionsAsyncTest {
    [Fact]
    public async Task SelectAsync_TaskResult_OnSuccess_TransformsValue() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.SelectAsync(x => x * 2);

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(20);
    }

    [Fact]
    public async Task SelectAsync_TaskResult_OnFailure_ReturnsFailure() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.SelectAsync(x => x * 2);

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public async Task SelectAsync_TaskResult_OnCancelled_ReturnsCancelled() {
        var cancelled = new Cancelled("user cancelled");
        var resultTask = Task.FromResult(OperationResult<int>.Cancelled(cancelled));

        var actual = await resultTask.SelectAsync(x => x * 2);

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.ShouldBe(cancelled);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_OnSuccess_TransformsValue() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.SelectAsync(x => Task.FromResult(x * 2));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(20);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_OnFailure_ReturnsFailure() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = await result.SelectAsync(x => Task.FromResult(x * 2));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public async Task SelectAsync_TaskResultAsyncSelector_OnSuccess_TransformsValue() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.SelectAsync(x => Task.FromResult(x * 2));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(20);
    }

    [Fact]
    public async Task BindAsync_TaskResult_OnSuccess_ReturnsBinderResult() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.BindAsync(x => OperationResult<string>.Success(x.ToString()));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("10");
    }

    [Fact]
    public async Task BindAsync_TaskResult_OnFailure_ReturnsFailure() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.BindAsync(x => OperationResult<string>.Success(x.ToString()));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_OnSuccess_ReturnsBinderResult() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.BindAsync(x => Task.FromResult(OperationResult<string>.Success(x.ToString())));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("10");
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_OnFailure_ReturnsFailure() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = await result.BindAsync(x => Task.FromResult(OperationResult<string>.Success(x.ToString())));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.ShouldBe(error);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncBinder_OnSuccess_ReturnsBinderResult() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.BindAsync(x => Task.FromResult(OperationResult<string>.Success(x.ToString())));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("10");
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnFailure_TransformsError() {
        var error = new Error("original error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.MapErrorAsync(e => new Error($"transformed: {e.Message}"));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnSuccess_ReturnsOriginal() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MapErrorAsync(e => new Error($"transformed: {e.Message}"));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(10);
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnFailure_TransformsError() {
        var error = new Error("original error");
        var result = OperationResult<int>.Failure(error);

        var actual = await result.MapErrorAsync(e => Task.FromResult(new Error($"transformed: {e.Message}")));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.MapErrorAsync(e => Task.FromResult(new Error($"transformed: {e.Message}")));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(10);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResultAsyncMapper_OnFailure_TransformsError() {
        var error = new Error("original error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.MapErrorAsync(e => Task.FromResult(new Error($"transformed: {e.Message}")));

        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResult_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var resultTask = Task.FromResult(OperationResult<int>.Cancelled(cancelled));

        var actual = await resultTask.MapCancellationAsync(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResult_OnSuccess_ReturnsOriginal() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MapCancellationAsync(c => new Cancelled($"transformed: {c.Reason}"));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(10);
    }

    [Fact]
    public async Task MapCancellationAsync_AsyncMapper_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = await result.MapCancellationAsync(c => Task.FromResult(new Cancelled($"transformed: {c.Reason}")));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public async Task MapCancellationAsync_AsyncMapper_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.MapCancellationAsync(c => Task.FromResult(new Cancelled($"transformed: {c.Reason}")));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(10);
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResultAsyncMapper_OnCancelled_TransformsCancellation() {
        var cancelled = new Cancelled("original reason");
        var resultTask = Task.FromResult(OperationResult<int>.Cancelled(cancelled));

        var actual = await resultTask.MapCancellationAsync(c => Task.FromResult(new Cancelled($"transformed: {c.Reason}")));

        actual.IsCancelled.ShouldBeTrue();
        actual.Cancellation.Reason.ShouldBe("transformed: original reason");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MatchAsync(x => $"success: {x}", _ => "failure", _ => "cancelled");

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.MatchAsync(x => $"success: {x}", e => $"failure: {e.Message}", _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var resultTask = Task.FromResult(OperationResult<int>.Cancelled(cancelled));

        var actual = await resultTask.MatchAsync(x => $"success: {x}", _ => "failure", c => $"cancelled: {c.Reason}");

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnSuccess_ExecutesSuccessHandler() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.MatchAsync(x => Task.FromResult($"success: {x}"), _ => "failure", _ => "cancelled");

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnFailure_DoesNotCallAsyncSuccess() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = await result.MatchAsync(x => Task.FromResult($"success: {x}"), e => $"failure: {e.Message}", _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = await result.MatchAsync(x => $"success: {x}", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnSuccess_DoesNotCallAsyncFailure() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.MatchAsync(x => $"success: {x}", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncCancelled_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = await result.MatchAsync(x => $"success: {x}", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_AsyncCancelled_OnSuccess_DoesNotCallAsyncCancelled() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.MatchAsync(x => $"success: {x}", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnSuccess_ExecutesSuccessHandler() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            _ => Task.FromResult("failure"),
            _ => Task.FromResult("cancelled")
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var result = OperationResult<int>.Failure(error);

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            e => Task.FromResult($"failure: {e.Message}"),
            _ => Task.FromResult("cancelled")
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            _ => Task.FromResult("failure"),
            c => Task.FromResult($"cancelled: {c.Reason}")
        );

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), _ => "failure", _ => "cancelled");

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnFailure_DoesNotCallAsyncSuccess() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), e => $"failure: {e.Message}", _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.MatchAsync(x => $"success: {x}", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnSuccess_DoesNotCallAsyncFailure() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MatchAsync(x => $"success: {x}", e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncCancelled_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var resultTask = Task.FromResult(OperationResult<int>.Cancelled(cancelled));

        var actual = await resultTask.MatchAsync(x => $"success: {x}", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncCancelled_OnSuccess_DoesNotCallAsyncCancelled() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MatchAsync(x => $"success: {x}", _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccessAndCancelled_OnSuccess_ExecutesSuccessHandler() {
        var result = OperationResult<int>.Success(10);

        var actual = await result.MatchAsync(x => Task.FromResult($"success: {x}"), _ => "failure", _ => Task.FromResult("cancelled"));

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccessAndCancelled_OnCancelled_ExecutesCancelledHandler() {
        var cancelled = new Cancelled("user cancelled");
        var result = OperationResult<int>.Cancelled(cancelled);

        var actual = await result.MatchAsync(x => Task.FromResult($"success: {x}"), _ => "failure", c => Task.FromResult($"cancelled: {c.Reason}"));

        actual.ShouldBe("cancelled: user cancelled");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccessAndFailure_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), _ => Task.FromResult("failure"), _ => "cancelled");

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccessAndFailure_OnFailure_ExecutesFailureHandler() {
        var error = new Error("test error");
        var resultTask = Task.FromResult(OperationResult<int>.Failure(error));

        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), e => Task.FromResult($"failure: {e.Message}"), _ => "cancelled");

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAllAsync_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult<int>.Success(10));

        var actual = await resultTask.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            _ => Task.FromResult("failure"),
            _ => Task.FromResult("cancelled")
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task SelectAsync_ChainedOperations_WorksCorrectly() {
        var result = OperationResult<int>.Success(10);

        var actual = await result
            .SelectAsync(x => Task.FromResult(x * 2))
            .SelectAsync(x => x + 5);

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(25);
    }

    [Fact]
    public async Task BindAsync_ChainedOperations_WorksCorrectly() {
        var result = OperationResult<int>.Success(10);

        var actual = await result
            .BindAsync(x => Task.FromResult(OperationResult<int>.Success(x * 2)))
            .BindAsync(x => OperationResult<int>.Success(x + 5));

        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(25);
    }
}

