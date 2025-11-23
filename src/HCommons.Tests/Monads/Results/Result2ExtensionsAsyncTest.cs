using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

public class Result2ExtensionsAsyncTest {
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
