using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

public class ResultExtensionsTest {
    [Fact]
    public void MapError_OnSuccess_ReturnsOriginalResult() {
        // Arrange
        var result = Result.Success();

        // Act
        var actual = result.MapError(e => new Error($"mapped: {e.Message}"));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapError_OnFailure_TransformsError() {
        // Arrange
        var error = new Error("original error");
        var result = Result.Failure(error);

        // Act
        var actual = result.MapError(e => new Error($"mapped: {e.Message}"));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("mapped: original error");
    }

    [Fact]
    public void MapError_WithState_OnSuccess_ReturnsOriginalResult() {
        // Arrange
        var result = Result.Success();
        var state = "my state";

        // Act
        var actual = result.MapError(state, (s, e) => new Error($"{s}: {e.Message}"));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
    }

    [Fact]
    public void MapError_WithState_OnFailure_TransformsErrorWithState() {
        // Arrange
        var error = new Error("original error");
        var result = Result.Failure(error);
        var state = "my state";

        // Act
        var actual = result.MapError(state, (s, e) => new Error($"{s}: {e.Message}"));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.Error.Message.ShouldBe("my state: original error");
    }

    [Fact]
    public void Match_OnSuccess_ExecutesSuccessFunction() {
        // Arrange
        var result = Result.Success();

        // Act
        var actual = result.Match(() => "success", _ => "failure");

        // Assert
        actual.ShouldBe("success");
    }

    [Fact]
    public void Match_OnFailure_ExecutesFailureFunction() {
        // Arrange
        var error = new Error("test error");
        var result = Result.Failure(error);

        // Act
        var actual = result.Match(() => "success", e => $"failure: {e.Message}");

        // Assert
        actual.ShouldBe("failure: test error");
    }

    [Fact]
    public void Match_WithState_OnSuccess_ExecutesSuccessFunctionWithState() {
        // Arrange
        var result = Result.Success();
        var state = "my state";

        // Act
        var actual = result.Match(state, s => $"success: {s}", (s, _) => $"failure: {s}");

        // Assert
        actual.ShouldBe("success: my state");
    }

    [Fact]
    public void Match_WithState_OnFailure_ExecutesFailureFunctionWithState() {
        // Arrange
        var error = new Error("test error");
        var result = Result.Failure(error);
        var state = "my state";

        // Act
        var actual = result.Match(state, s => $"success: {s}", (s, e) => $"failure: {s}, {e.Message}");

        // Assert
        actual.ShouldBe("failure: my state, test error");
    }

    [Fact]
    public void Switch_OnSuccess_ExecutesSuccessAction() {
        // Arrange
        var result = Result.Success();
        var executed = "";

        // Act
        result.Switch(() => executed = "success", _ => executed = "failure");

        // Assert
        executed.ShouldBe("success");
    }

    [Fact]
    public void Switch_OnFailure_ExecutesFailureAction() {
        // Arrange
        var error = new Error("test error");
        var result = Result.Failure(error);
        var executed = "";

        // Act
        result.Switch(() => executed = "success", e => executed = $"failure: {e.Message}");

        // Assert
        executed.ShouldBe("failure: test error");
    }

    [Fact]
    public void Switch_WithState_OnSuccess_ExecutesSuccessActionWithState() {
        // Arrange
        var result = Result.Success();
        var state = "my state";
        var executed = "";

        // Act
        result.Switch(state, s => executed = $"success: {s}", (s, _) => executed = $"failure: {s}");

        // Assert
        executed.ShouldBe("success: my state");
    }

    [Fact]
    public void Switch_WithState_OnFailure_ExecutesFailureActionWithState() {
        // Arrange
        var error = new Error("test error");
        var result = Result.Failure(error);
        var state = "my state";
        var executed = "";

        // Act
        result.Switch(state, s => executed = $"success: {s}", (s, e) => executed = $"failure: {s}, {e.Message}");

        // Assert
        executed.ShouldBe("failure: my state, test error");
    }
}
