using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

public class Result2ExtensionsTest {
    [Fact]
    public void Select_OnSuccess_TransformsSuccessValue() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = result.Select(s => s.Length);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public void Select_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = result.Select(s => s.Length);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public void Select_WithState_OnSuccess_TransformsSuccessValueWithState() {
        // Arrange
        var result = Result<string, int>.Success("test");
        var state = 10;

        // Act
        var actual = result.Select(state, (st, s) => s.Length + st);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(14);
    }

    [Fact]
    public void Select_WithState_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var result = Result<string, int>.Failure(42);
        var state = 10;

        // Act
        var actual = result.Select(state, (st, s) => s.Length + st);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public void Bind_OnSuccess_BindsToNewResult() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = result.Bind(s => Result<int, int>.Success(s.Length));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(4);
    }

    [Fact]
    public void Bind_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = result.Bind(s => Result<int, int>.Success(s.Length));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public void Bind_WithState_OnSuccess_BindsToNewResultWithState() {
        // Arrange
        var result = Result<string, int>.Success("test");
        var state = 10;

        // Act
        var actual = result.Bind(state, (st, s) => Result<int, int>.Success(s.Length + st));

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe(14);
    }

    [Fact]
    public void Bind_WithState_OnFailure_ReturnsOriginalFailure() {
        // Arrange
        var result = Result<string, int>.Failure(42);
        var state = 10;

        // Act
        var actual = result.Bind(state, (st, s) => Result<int, int>.Success(s.Length + st));

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public void MapError_OnSuccess_ReturnsOriginalSuccess() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = result.MapError(f => f * 2);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("test");
    }

    [Fact]
    public void MapError_OnFailure_TransformsFailureValue() {
        // Arrange
        var result = Result<string, int>.Failure(21);

        // Act
        var actual = result.MapError(f => f * 2);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public void MapError_WithState_OnSuccess_ReturnsOriginalSuccess() {
        // Arrange
        var result = Result<string, int>.Success("test");
        var state = 10;

        // Act
        var actual = result.MapError(state, (st, f) => f + st);

        // Assert
        actual.IsSuccess.ShouldBeTrue();
        actual.Value.ShouldBe("test");
    }

    [Fact]
    public void MapError_WithState_OnFailure_TransformsFailureValueWithState() {
        // Arrange
        var result = Result<string, int>.Failure(32);
        var state = 10;

        // Act
        var actual = result.MapError(state, (st, f) => f + st);

        // Assert
        actual.IsFailure.ShouldBeTrue();
        actual.FailureValue.ShouldBe(42);
    }

    [Fact]
    public void Match_OnSuccess_ExecutesSuccessFunction() {
        // Arrange
        var result = Result<string, int>.Success("test");

        // Act
        var actual = result.Match(s => $"success: {s}", f => $"failure: {f}");

        // Assert
        actual.ShouldBe("success: test");
    }

    [Fact]
    public void Match_OnFailure_ExecutesFailureFunction() {
        // Arrange
        var result = Result<string, int>.Failure(42);

        // Act
        var actual = result.Match(s => $"success: {s}", f => $"failure: {f}");

        // Assert
        actual.ShouldBe("failure: 42");
    }

    [Fact]
    public void Match_WithState_OnSuccess_ExecutesSuccessFunctionWithState() {
        // Arrange
        var result = Result<string, int>.Success("test");
        var state = "my state";

        // Act
        var actual = result.Match(state, (st, s) => $"success: {st}, {s}", (st, f) => $"failure: {st}, {f}");

        // Assert
        actual.ShouldBe("success: my state, test");
    }

    [Fact]
    public void Match_WithState_OnFailure_ExecutesFailureFunctionWithState() {
        // Arrange
        var result = Result<string, int>.Failure(42);
        var state = "my state";

        // Act
        var actual = result.Match(state, (st, s) => $"success: {st}, {s}", (st, f) => $"failure: {st}, {f}");

        // Assert
        actual.ShouldBe("failure: my state, 42");
    }

    [Fact]
    public void Switch_OnSuccess_ExecutesSuccessAction() {
        // Arrange
        var result = Result<string, int>.Success("test");
        var executed = "";

        // Act
        result.Switch(s => executed = $"success: {s}", f => executed = $"failure: {f}");

        // Assert
        executed.ShouldBe("success: test");
    }

    [Fact]
    public void Switch_OnFailure_ExecutesFailureAction() {
        // Arrange
        var result = Result<string, int>.Failure(42);
        var executed = "";

        // Act
        result.Switch(s => executed = $"success: {s}", f => executed = $"failure: {f}");

        // Assert
        executed.ShouldBe("failure: 42");
    }

    [Fact]
    public void Switch_WithState_OnSuccess_ExecutesSuccessActionWithState() {
        // Arrange
        var result = Result<string, int>.Success("test");
        var state = "my state";
        var executed = "";

        // Act
        result.Switch(state, (st, s) => executed = $"success: {st}, {s}", (st, f) => executed = $"failure: {st}, {f}");

        // Assert
        executed.ShouldBe("success: my state, test");
    }

    [Fact]
    public void Switch_WithState_OnFailure_ExecutesFailureActionWithState() {
        // Arrange
        var result = Result<string, int>.Failure(42);
        var state = "my state";
        var executed = "";

        // Act
        result.Switch(state, (st, s) => executed = $"success: {st}, {s}", (st, f) => executed = $"failure: {st}, {f}");

        // Assert
        executed.ShouldBe("failure: my state, 42");
    }
}
