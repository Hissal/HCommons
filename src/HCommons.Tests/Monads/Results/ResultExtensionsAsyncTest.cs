using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

public class ResultExtensionsAsyncTest {
    [Fact]
    public async Task MatchAsync_TaskResult_OnSuccess_ExecutesSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result.Success());

        // Act
        var actual = await resultTask.MatchAsync(() => "success", _ => "failure");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResult_OnFailure_ExecutesFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var resultTask = Task.FromResult(Result.Failure(error));

        // Act
        var actual = await resultTask.MatchAsync(() => "success", e => $"failure: {e.Message}");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var result = Result.Success();

        // Act
        var actual = await result.MatchAsync(async () => {
            await Task.Delay(1);
            return "success";
        }, _ => "failure");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AsyncSuccess_OnFailure_ExecutesSyncFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var result = Result.Failure(error);

        // Act
        var actual = await result.MatchAsync(async () => {
            await Task.Delay(1);
            return "success";
        }, e => $"failure: {e.Message}");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnSuccess_ExecutesSyncSuccessFunction() {
        // Arrange
        var result = Result.Success();

        // Act
        var actual = await result.MatchAsync(() => "success", async e => {
            await Task.Delay(1);
            return $"failure: {e.Message}";
        });

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_AsyncFailure_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var result = Result.Failure(error);

        // Act
        var actual = await result.MatchAsync(() => "success", async e => {
            await Task.Delay(1);
            return $"failure: {e.Message}";
        });

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result.Success());

        // Act
        var actual = await resultTask.MatchAsync(async () => {
            await Task.Delay(1);
            return "success";
        }, _ => "failure");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncSuccess_OnFailure_ExecutesSyncFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var resultTask = Task.FromResult(Result.Failure(error));

        // Act
        var actual = await resultTask.MatchAsync(async () => {
            await Task.Delay(1);
            return "success";
        }, e => $"failure: {e.Message}");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnSuccess_ExecutesSyncSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result.Success());

        // Act
        var actual = await resultTask.MatchAsync(() => "success", async e => {
            await Task.Delay(1);
            return $"failure: {e.Message}";
        });

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncFailure_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var resultTask = Task.FromResult(Result.Failure(error));

        // Act
        var actual = await resultTask.MatchAsync(() => "success", async e => {
            await Task.Delay(1);
            return $"failure: {e.Message}";
        });

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_BothAsync_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var result = Result.Success();

        // Act
        var actual = await result.MatchAsync(
            async () => {
                await Task.Delay(1);
                return "success";
            },
            async e => {
                await Task.Delay(1);
                return $"failure: {e.Message}";
            });

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_BothAsync_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var result = Result.Failure(error);

        // Act
        var actual = await result.MatchAsync(
            async () => {
                await Task.Delay(1);
                return "success";
            },
            async e => {
                await Task.Delay(1);
                return $"failure: {e.Message}";
            });

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public async Task MatchAsync_TaskResultBothAsync_OnSuccess_ExecutesAsyncSuccessFunction() {
        // Arrange
        var resultTask = Task.FromResult(Result.Success());

        // Act
        var actual = await resultTask.MatchAsync(
            async () => {
                await Task.Delay(1);
                return "success";
            },
            async e => {
                await Task.Delay(1);
                return $"failure: {e.Message}";
            });

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public async Task MatchAsync_TaskResultBothAsync_OnFailure_ExecutesAsyncFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var resultTask = Task.FromResult(Result.Failure(error));

        // Act
        var actual = await resultTask.MatchAsync(
            async () => {
                await Task.Delay(1);
                return "success";
            },
            async e => {
                await Task.Delay(1);
                return $"failure: {e.Message}";
            });

        // Assert
        actual.ShouldBe("failure: test error");
    }
}
