using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(Result1Extensions))]
public class Result1ExtensionsTest {

    [Fact]
    public void Match_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        
        var output = result.Match(
            value => value * 2,
            _ => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public void Match_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var output = result.Match(
            value => value * 2,
            error => error.Message.Length
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public void Match_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, _) => state
        );
        
        output.ShouldBe(52);
    }

    [Fact]
    public void Match_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var output = result.Match(
            10,
            (state, value) => value + state,
            (state, error) => state + error.Message.Length
        );
        
        output.ShouldBe(20);
    }

    [Fact]
    public void Switch_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeTrue();
        failureCalled.ShouldBeFalse();
    }

    [Fact]
    public void Switch_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        var successCalled = false;
        var failureCalled = false;
        
        result.Switch(
            _ => successCalled = true,
            _ => failureCalled = true
        );
        
        successCalled.ShouldBeFalse();
        failureCalled.ShouldBeTrue();
    }

    [Fact]
    public void Switch_WithState_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, _) => counter = state
        );
        
        counter.ShouldBe(142);
    }

    [Fact]
    public void Switch_WithState_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        var counter = 0;
        
        result.Switch(
            100,
            (state, value) => counter = state + value,
            (state, error) => counter = state + error.Message.Length
        );
        
        counter.ShouldBe(110);
    }

    [Fact]
    public void Select_WhenSuccess_TransformsValue() {
        var result = Result<int>.Success(42);
        
        var mapped = result.Select(x => x * 2);
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(84);
    }

    [Fact]
    public void Select_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        var mapped = result.Select(x => x * 2);
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.ShouldBe(error);
    }

    [Fact]
    public void Select_WithState_WhenSuccess_TransformsValue() {
        var result = Result<int>.Success(42);
        
        var mapped = result.Select(10, (state, value) => value + state);
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(52);
    }

    [Fact]
    public void Select_WithState_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        var mapped = result.Select(10, (state, value) => value + state);
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.ShouldBe(error);
    }

    [Fact]
    public void Bind_WhenSuccess_ExecutesBinder() {
        var result = Result<int>.Success(42);
        
        var bound = result.Bind(x => Result<string>.Success(x.ToString()));
        
        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("42");
    }

    [Fact]
    public void Bind_WhenSuccess_BinderReturnsFailure() {
        var result = Result<int>.Success(42);
        var error = new Error("Binder error");
        
        var bound = result.Bind(_ => Result<string>.Failure(error));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public void Bind_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        var bound = result.Bind(x => Result<string>.Success(x.ToString()));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public void Bind_WithState_WhenSuccess_ExecutesBinder() {
        var result = Result<int>.Success(42);
        
        var bound = result.Bind("prefix", (state, value) => Result<string>.Success($"{state}:{value}"));
        
        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("prefix:42");
    }

    [Fact]
    public void Bind_WithState_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        var bound = result.Bind("prefix", (state, value) => Result<string>.Success($"{state}:{value}"));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public void MapError_WhenSuccess_DoesNotTransform() {
        var result = Result<int>.Success(42);
        
        var mapped = result.MapError(_ => new Error("Mapped error"));
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(42);
    }

    [Fact]
    public void MapError_WhenFailure_TransformsError() {
        var result = Result<int>.Failure(new Error("Original error"));
        
        var mapped = result.MapError(_ => new Error("Mapped error"));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.Message.ShouldBe("Mapped error");
    }

    [Fact]
    public void MapError_WithState_WhenSuccess_DoesNotTransform() {
        var result = Result<int>.Success(42);
        
        var mapped = result.MapError("prefix", (_, _) => new Error("Should not be called"));
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(42);
    }

    [Fact]
    public void MapError_WithState_WhenFailure_TransformsError() {
        var result = Result<int>.Failure(new Error("Original error"));
        
        var mapped = result.MapError("prefix", (state, e) => new Error($"{state}:{e.Message}"));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.Message.ShouldBe("prefix:Original error");
    }
}

