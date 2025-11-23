using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

public class Result2ExtensionsAsyncTest {
    [Fact]
    public async Task SelectAsync_TaskResult_OnSuccess_TransformsSuccessValue() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.SelectAsync(s => s.Length);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public async Task SelectAsync_TaskResult_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.SelectAsync(s => s.Length);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_OnSuccess_TransformsSuccessValue() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = await result.SelectAsync(s => Task.FromResult(s.Length));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = await result.SelectAsync(s => Task.FromResult(s.Length));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task SelectAsync_TaskResultAsyncSelector_OnSuccess_TransformsSuccessValue() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.SelectAsync(s => Task.FromResult(s.Length));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public async Task SelectAsync_TaskResultAsyncSelector_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.SelectAsync(s => Task.FromResult(s.Length));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task BindAsync_TaskResult_OnSuccess_BindsToNewResult() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.BindAsync(s => Result<int, int>.Success(s.Length));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public async Task BindAsync_TaskResult_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.BindAsync(s => Result<int, int>.Success(s.Length));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_OnSuccess_BindsToNewResult() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = await result.BindAsync(s => Task.FromResult(Result<int, int>.Success(s.Length)));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = await result.BindAsync(s => Task.FromResult(Result<int, int>.Success(s.Length)));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncBinder_OnSuccess_BindsToNewResult() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.BindAsync(s => Task.FromResult(Result<int, int>.Success(s.Length)));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncBinder_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.BindAsync(s => Task.FromResult(Result<int, int>.Success(s.Length)));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnSuccess_ReturnsOriginalSuccess() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.MapErrorAsync(f => f * 2);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("test");
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_OnFailure_TransformsFailureValue() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(21));

        // Act
        var actual = await resultTask.MapErrorAsync(f => f * 2);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnSuccess_ReturnsOriginalSuccess() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = await result.MapErrorAsync(f => Task.FromResult(f * 2));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("test");
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_OnFailure_TransformsFailureValue() {
        // Arrange
        var result = Result<string, int>.Failure(21);

        // Act
        var actual = await result.MapErrorAsync(f => Task.FromResult(f * 2));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResultAsyncMapper_OnSuccess_ReturnsOriginalSuccess() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.MapErrorAsync(f => Task.FromResult(f * 2));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("test");
    }

    [Fact]
    public async Task MapErrorAsync_TaskResultAsyncMapper_OnFailure_TransformsFailureValue() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(21));

        // Act
        var actual = await resultTask.MapErrorAsync(f => Task.FromResult(f * 2));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public async Task MatchAsync_TaskResult_OnSuccess_ExecutesSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.MatchAsync(s => $"success: {s}", f => $"failure: {f}");

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public async Task MatchAsync_TaskResult_OnFailure_ExecutesFailureFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.MatchAsync(s => $"success: {s}", f => $"failure: {f}");

        // Assert
        actual.ShouldBe("failure: 42");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = await result.MatchAsync(async s => {
            await Task.Delay(1);
            return $"success: {s}";
        }, f => $"failure: {f}");

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnFailure_ExecutesSyncFailureFunction() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = await result.MatchAsync(async s => {
            await Task.Delay(1);
            return $"success: {s}";
        }, f => $"failure: {f}");

        // Assert
        actual.ShouldBe("failure: 42");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnSuccess_ExecutesSyncSuccessFunction() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = await result.MatchAsync(s => $"success: {s}", async f => {
            await Task.Delay(1);
            return $"failure: {f}";
        });

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = await result.MatchAsync(s => $"success: {s}", async f => {
            await Task.Delay(1);
            return $"failure: {f}";
        });

        // Assert
        actual.ShouldBe("failure: 42");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.MatchAsync(async s => {
            await Task.Delay(1);
            return $"success: {s}";
        }, f => $"failure: {f}");

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnFailure_ExecutesSyncFailureFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.MatchAsync(async s => {
            await Task.Delay(1);
            return $"success: {s}";
        }, f => $"failure: {f}");

        // Assert
        actual.ShouldBe("failure: 42");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnSuccess_ExecutesSyncSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.MatchAsync(s => $"success: {s}", async f => {
            await Task.Delay(1);
            return $"failure: {f}";
        });

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.MatchAsync(s => $"success: {s}", async f => {
            await Task.Delay(1);
            return $"failure: {f}";
        });

        // Assert
        actual.ShouldBe("failure: 42");
    }

    [Fact]
    public async Task MatchAsync_BothAsync_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = await result.MatchAsync(
            async s => {
                await Task.Delay(1);
                return $"success: {s}";
            },
            async f => {
                await Task.Delay(1);
                return $"failure: {f}";
            });

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public async Task MatchAsync_BothAsync_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = await result.MatchAsync(
            async s => {
                await Task.Delay(1);
                return $"success: {s}";
            },
            async f => {
                await Task.Delay(1);
                return $"failure: {f}";
            });

        // Assert
        actual.ShouldBe("failure: 42");
    }

    [Fact]
    public async Task MatchAsync_TaskResultBothAsync_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Success("test"));

        // Act
        var actual = await resultTask.MatchAsync(
            async s => {
                await Task.Delay(1);
                return $"success: {s}";
            },
            async f => {
                await Task.Delay(1);
                return $"failure: {f}";
            });

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public async Task MatchAsync_TaskResultBothAsync_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result<string, int>.Failure(42));

        // Act
        var actual = await resultTask.MatchAsync(
            async s => {
                await Task.Delay(1);
                return $"success: {s}";
            },
            async f => {
                await Task.Delay(1);
                return $"failure: {f}";
            });

        // Assert
        actual.ShouldBe("failure: 42");
    }
}
