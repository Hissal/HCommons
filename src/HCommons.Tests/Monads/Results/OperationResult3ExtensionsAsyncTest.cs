using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(OperationResult3ExtensionsAsync))]
public class OperationResult3ExtensionsAsyncTest {
    [Fact]
    public async Task SelectAsync_TaskResult_OnSuccess_TransformsSuccessValue() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.SelectAsync(x => x * 2);

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(20);
    }

    [Fact]
    public async Task SelectAsync_TaskResult_OnFailure_ReturnsFailure() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("test error"));

        var actual = await resultTask.SelectAsync(x => x * 2);

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public async Task SelectAsync_TaskResult_OnCancelled_ReturnsCancelled() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Cancelled(true));

        var actual = await resultTask.SelectAsync(x => x * 2);

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(true);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_OnSuccess_TransformsSuccessValue() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.SelectAsync(x => Task.FromResult(x * 2));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(20);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_OnFailure_ReturnsFailure() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = await result.SelectAsync(x => Task.FromResult(x * 2));

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public async Task SelectAsync_TaskResultAsyncSelector_OnSuccess_TransformsSuccessValue() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.SelectAsync(x => Task.FromResult(x * 2));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(20);
    }

    [Fact]
    public async Task BindAsync_TaskResult_OnSuccess_ReturnsBinderResult() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.BindAsync(x => OperationResult<double, string, bool>.Success(x / 2.0));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(5.0);
    }

    [Fact]
    public async Task BindAsync_TaskResult_OnFailure_ReturnsFailure() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("test error"));

        var actual = await resultTask.BindAsync(x => OperationResult<double, string, bool>.Success(x / 2.0));

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_OnSuccess_ReturnsBinderResult() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.BindAsync(x => Task.FromResult(OperationResult<double, string, bool>.Success(x / 2.0)));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(5.0);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_OnFailure_ReturnsFailure() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = await result.BindAsync(x => Task.FromResult(OperationResult<double, string, bool>.Success(x / 2.0)));

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("test error");
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncBinder_OnSuccess_ReturnsBinderResult() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.BindAsync(x => Task.FromResult(OperationResult<double, string, bool>.Success(x / 2.0)));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(5.0);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnFailure_TransformsFailureValue() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("original error"));

        var actual = await resultTask.MapErrorAsync(f => $"transformed: {f}");

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnSuccess_ReturnsOriginal() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.MapErrorAsync(f => $"transformed: {f}");

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnFailure_TransformsFailureValue() {
        var result = OperationResult<int, string, bool>.Failure("original error");

        var actual = await result.MapErrorAsync(f => Task.FromResult($"transformed: {f}"));

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.MapErrorAsync(f => Task.FromResult($"transformed: {f}"));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResultAsyncMapper_OnFailure_TransformsFailureValue() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("original error"));

        var actual = await resultTask.MapErrorAsync(f => Task.FromResult($"transformed: {f}"));

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("transformed: original error");
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResult_OnCancelled_TransformsCancellationValue() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Cancelled(true));

        var actual = await resultTask.MapCancellationAsync(c => !c);

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(false);
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResult_OnSuccess_ReturnsOriginal() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.MapCancellationAsync(c => !c);

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public async Task MapCancellationAsync_AsyncMapper_OnCancelled_TransformsCancellationValue() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = await result.MapCancellationAsync(c => Task.FromResult(!c));

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(false);
    }

    [Fact]
    public async Task MapCancellationAsync_AsyncMapper_OnSuccess_ReturnsOriginal() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.MapCancellationAsync(c => Task.FromResult(!c));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(10);
    }

    [Fact]
    public async Task MapCancellationAsync_TaskResultAsyncMapper_OnCancelled_TransformsCancellationValue() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Cancelled(true));

        var actual = await resultTask.MapCancellationAsync(c => Task.FromResult(!c));

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe(false);
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.MatchAsync(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnFailure_ExecutesFailureHandler() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("test error"));

        var actual = await resultTask.MatchAsync(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultSyncHandlers_OnCancelled_ExecutesCancelledHandler() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Cancelled(true));

        var actual = await resultTask.MatchAsync(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("cancelled: True");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnSuccess_ExecutesSuccessHandler() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnFailure_DoesNotCallAsyncSuccess() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            f => $"failure: {f}",
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnFailure_ExecutesFailureHandler() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = await result.MatchAsync(
            x => $"success: {x}",
            f => Task.FromResult($"failure: {f}"),
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnSuccess_DoesNotCallAsyncFailure() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.MatchAsync(
            x => $"success: {x}",
            f => Task.FromResult($"failure: {f}"),
            c => $"cancelled: {c}"
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncCancelled_OnCancelled_ExecutesCancelledHandler() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = await result.MatchAsync(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => Task.FromResult($"cancelled: {c}")
        );

        actual.ShouldBe("cancelled: True");
    }

    [Fact]
    public async Task MatchAsync_AsyncCancelled_OnSuccess_DoesNotCallAsyncCancelled() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.MatchAsync(
            x => $"success: {x}",
            f => $"failure: {f}",
            c => Task.FromResult($"cancelled: {c}")
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnSuccess_ExecutesSuccessHandler() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            f => Task.FromResult($"failure: {f}"),
            c => Task.FromResult($"cancelled: {c}")
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnFailure_ExecutesFailureHandler() {
        var result = OperationResult<int, string, bool>.Failure("test error");

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            f => Task.FromResult($"failure: {f}"),
            c => Task.FromResult($"cancelled: {c}")
        );

        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AllAsync_OnCancelled_ExecutesCancelledHandler() {
        var result = OperationResult<int, string, bool>.Cancelled(true);

        var actual = await result.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            f => Task.FromResult($"failure: {f}"),
            c => Task.FromResult($"cancelled: {c}")
        );

        actual.ShouldBe("cancelled: True");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnSuccess_ExecutesSuccessHandler() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        // Act
        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), f => $"failure: {f}", c => $"cancelled: {c}");

        // Assert
        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnFailure_DoesNotCallAsyncSuccess() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("error"));

        // Act
        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), f => $"failure: {f}", c => $"cancelled: {c}");

        // Assert
        actual.ShouldBe("failure: error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnFailure_ExecutesFailureHandler() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("error"));

        // Act
        var actual = await resultTask.MatchAsync(x => $"success: {x}", f => Task.FromResult($"failure: {f}"), c => $"cancelled: {c}");

        // Assert
        actual.ShouldBe("failure: error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnSuccess_DoesNotCallAsyncFailure() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        // Act
        var actual = await resultTask.MatchAsync(x => $"success: {x}", f => Task.FromResult($"failure: {f}"), c => $"cancelled: {c}");

        // Assert
        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncCancelled_OnCancelled_ExecutesCancelledHandler() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Cancelled(true));

        // Act
        var actual = await resultTask.MatchAsync(x => $"success: {x}", f => $"failure: {f}", c => Task.FromResult($"cancelled: {c}"));

        // Assert
        actual.ShouldBe("cancelled: True");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncCancelled_OnSuccess_DoesNotCallAsyncCancelled() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        // Act
        var actual = await resultTask.MatchAsync(x => $"success: {x}", f => $"failure: {f}", c => Task.FromResult($"cancelled: {c}"));

        // Assert
        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccessAndCancelled_OnSuccess_ExecutesSuccessHandler() {
        // Arrange
        var result = OperationResult<int, string, bool>.Success(10);

        // Act
        var actual = await result.MatchAsync(x => Task.FromResult($"success: {x}"), f => $"failure: {f}", c => Task.FromResult($"cancelled: {c}"));

        // Assert
        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccessAndCancelled_OnCancelled_ExecutesCancelledHandler() {
        // Arrange
        var result = OperationResult<int, string, bool>.Cancelled(true);

        // Act
        var actual = await result.MatchAsync(x => Task.FromResult($"success: {x}"), f => $"failure: {f}", c => Task.FromResult($"cancelled: {c}"));

        // Assert
        actual.ShouldBe("cancelled: True");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccessAndFailure_OnSuccess_ExecutesSuccessHandler() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        // Act
        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), f => Task.FromResult($"failure: {f}"), c => $"cancelled: {c}");

        // Assert
        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccessAndFailure_OnFailure_ExecutesFailureHandler() {
        // Arrange
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Failure("error"));

        // Act
        var actual = await resultTask.MatchAsync(x => Task.FromResult($"success: {x}"), f => Task.FromResult($"failure: {f}"), c => $"cancelled: {c}");

        // Assert
        actual.ShouldBe("failure: error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAllAsync_OnSuccess_ExecutesSuccessHandler() {
        var resultTask = Task.FromResult(OperationResult<int, string, bool>.Success(10));

        var actual = await resultTask.MatchAsync(
            x => Task.FromResult($"success: {x}"),
            f => Task.FromResult($"failure: {f}"),
            c => Task.FromResult($"cancelled: {c}")
        );

        actual.ShouldBe("success: 10");
    }

    [Fact]
    public async Task SelectAsync_ChainedOperations_WorksCorrectly() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result
            .SelectAsync(x => Task.FromResult(x * 2))
            .SelectAsync(x => x + 5);

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(25);
    }

    [Fact]
    public async Task BindAsync_ChainedOperations_WorksCorrectly() {
        var result = OperationResult<int, string, bool>.Success(10);

        var actual = await result
            .BindAsync(x => Task.FromResult(OperationResult<int, string, bool>.Success(x * 2)))
            .BindAsync(x => OperationResult<int, string, bool>.Success(x + 5));

        actual.IsSuccess.ShouldBeTrue();
        actual.SuccessValue.ShouldBe(25);
    }

    [Fact]
    public async Task MapErrorAsync_ChainedOperations_WorksCorrectly() {
        var result = OperationResult<int, string, bool>.Failure("original");

        var actual = await result
            .MapErrorAsync(f => Task.FromResult($"{f}1"))
            .MapErrorAsync(f => $"{f}2");

        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe("original12");
    }

    [Fact]
    public async Task MapCancellationAsync_ChainedOperations_WorksCorrectly() {
        var result = OperationResult<int, string, string>.Cancelled("original");

        var actual = await result
            .MapCancellationAsync(c => Task.FromResult($"{c}1"))
            .MapCancellationAsync(c => $"{c}2");

        actual.IsCancelled.ShouldBeTrue();
        actual.CancelledValue.ShouldBe("original12");
    }
}

