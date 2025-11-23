using HCommons.Monads;

namespace HCommons.Tests.Monads.Results;

[TestSubject(typeof(Result1ExtensionsAsync))]
public class Result1ExtensionsAsyncTest {

    [Fact]
    public async Task SelectAsync_TaskResult_WhenSuccess_TransformsValue() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var mapped = await resultTask.SelectAsync(x => x * 2);
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(84);
    }

    [Fact]
    public async Task SelectAsync_TaskResult_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var resultTask = Task.FromResult(Result<int>.Failure(error));
        
        var mapped = await resultTask.SelectAsync(x => x * 2);
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.ShouldBe(error);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_WhenSuccess_TransformsValue() {
        var result = Result<int>.Success(42);
        
        var mapped = await result.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(84);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        var mapped = await result.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.ShouldBe(error);
    }

    [Fact]
    public async Task SelectAsync_TaskResultAsyncSelector_WhenSuccess_TransformsValue() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var mapped = await resultTask.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(84);
    }

    [Fact]
    public async Task SelectAsync_TaskResultAsyncSelector_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var resultTask = Task.FromResult(Result<int>.Failure(error));
        
        var mapped = await resultTask.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.ShouldBe(error);
    }

    [Fact]
    public async Task BindAsync_TaskResult_WhenSuccess_ExecutesBinder() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var bound = await resultTask.BindAsync(x => Result<string>.Success(x.ToString()));
        
        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("42");
    }

    [Fact]
    public async Task BindAsync_TaskResult_WhenSuccess_BinderReturnsFailure() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        var error = new Error("Binder error");
        
        var bound = await resultTask.BindAsync(_ => Result<string>.Failure(error));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public async Task BindAsync_TaskResult_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var resultTask = Task.FromResult(Result<int>.Failure(error));
        
        var bound = await resultTask.BindAsync(x => Result<string>.Success(x.ToString()));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_WhenSuccess_ExecutesBinder() {
        var result = Result<int>.Success(42);
        
        var bound = await result.BindAsync(x => Task.FromResult(Result<string>.Success(x.ToString())));
        
        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("42");
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_WhenSuccess_BinderReturnsFailure() {
        var result = Result<int>.Success(42);
        var error = new Error("Binder error");
        
        var bound = await result.BindAsync(_ => Task.FromResult(Result<string>.Failure(error)));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var result = Result<int>.Failure(error);
        
        var bound = await result.BindAsync(x => Task.FromResult(Result<string>.Success(x.ToString())));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncBinder_WhenSuccess_ExecutesBinder() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var bound = await resultTask.BindAsync(x => Task.FromResult(Result<string>.Success(x.ToString())));
        
        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("42");
    }

    [Fact]
    public async Task BindAsync_TaskResultAsyncBinder_WhenFailure_PropagatesError() {
        var error = new Error("Test error");
        var resultTask = Task.FromResult(Result<int>.Failure(error));
        
        var bound = await resultTask.BindAsync(x => Task.FromResult(Result<string>.Success(x.ToString())));
        
        bound.IsFailure.ShouldBeTrue();
        bound.Error.ShouldBe(error);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_WhenSuccess_DoesNotTransform() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var mapped = await resultTask.MapErrorAsync(_ => new Error("Mapped error"));
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(42);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResult_WhenFailure_TransformsError() {
        var resultTask = Task.FromResult(Result<int>.Failure(new Error("Original error")));
        
        var mapped = await resultTask.MapErrorAsync(_ => new Error("Mapped error"));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.Message.ShouldBe("Mapped error");
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_WhenSuccess_DoesNotTransform() {
        var result = Result<int>.Success(42);
        
        var mapped = await result.MapErrorAsync(_ => Task.FromResult(new Error("Mapped error")));
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(42);
    }

    [Fact]
    public async Task MapErrorAsync_AsyncMapper_WhenFailure_TransformsError() {
        var result = Result<int>.Failure(new Error("Original error"));
        
        var mapped = await result.MapErrorAsync(_ => Task.FromResult(new Error("Mapped error")));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.Message.ShouldBe("Mapped error");
    }

    [Fact]
    public async Task MapErrorAsync_TaskResultAsyncMapper_WhenSuccess_DoesNotTransform() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var mapped = await resultTask.MapErrorAsync(_ => Task.FromResult(new Error("Mapped error")));
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(42);
    }

    [Fact]
    public async Task MapErrorAsync_TaskResultAsyncMapper_WhenFailure_TransformsError() {
        var resultTask = Task.FromResult(Result<int>.Failure(new Error("Original error")));
        
        var mapped = await resultTask.MapErrorAsync(_ => Task.FromResult(new Error("Mapped error")));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.Message.ShouldBe("Mapped error");
    }

    [Fact]
    public async Task MatchAsync_TaskResult_WhenSuccess_ExecutesOnSuccess() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var output = await resultTask.MatchAsync(
            value => value * 2,
            _ => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskResult_WhenFailure_ExecutesOnFailure() {
        var resultTask = Task.FromResult(Result<int>.Failure(new Error("Test error")));
        
        var output = await resultTask.MatchAsync(
            value => value * 2,
            error => error.Message.Length
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnSuccess_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        
        var output = await result.MatchAsync(
            value => Task.FromResult(value * 2),
            _ => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnSuccess_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var output = await result.MatchAsync(
            value => Task.FromResult(value * 2),
            error => error.Message.Length
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnFailure_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        
        var output = await result.MatchAsync(
            value => value * 2,
            _ => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnFailure_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var output = await result.MatchAsync(
            value => value * 2,
            error => Task.FromResult(error.Message.Length)
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncOnSuccess_WhenSuccess_ExecutesOnSuccess() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var output = await resultTask.MatchAsync(
            value => Task.FromResult(value * 2),
            _ => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncOnSuccess_WhenFailure_ExecutesOnFailure() {
        var resultTask = Task.FromResult(Result<int>.Failure(new Error("Test error")));
        
        var output = await resultTask.MatchAsync(
            value => Task.FromResult(value * 2),
            error => error.Message.Length
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncOnFailure_WhenSuccess_ExecutesOnSuccess() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var output = await resultTask.MatchAsync(
            value => value * 2,
            _ => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskResultAsyncOnFailure_WhenFailure_ExecutesOnFailure() {
        var resultTask = Task.FromResult(Result<int>.Failure(new Error("Test error")));
        
        var output = await resultTask.MatchAsync(
            value => value * 2,
            error => Task.FromResult(error.Message.Length)
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public async Task MatchAsync_BothAsync_WhenSuccess_ExecutesOnSuccess() {
        var result = Result<int>.Success(42);
        
        var output = await result.MatchAsync(
            value => Task.FromResult(value * 2),
            _ => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_BothAsync_WhenFailure_ExecutesOnFailure() {
        var result = Result<int>.Failure(new Error("Test error"));
        
        var output = await result.MatchAsync(
            value => Task.FromResult(value * 2),
            error => Task.FromResult(error.Message.Length)
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public async Task MatchAsync_TaskResultBothAsync_WhenSuccess_ExecutesOnSuccess() {
        var resultTask = Task.FromResult(Result<int>.Success(42));
        
        var output = await resultTask.MatchAsync(
            value => Task.FromResult(value * 2),
            _ => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskResultBothAsync_WhenFailure_ExecutesOnFailure() {
        var resultTask = Task.FromResult(Result<int>.Failure(new Error("Test error")));
        
        var output = await resultTask.MatchAsync(
            value => Task.FromResult(value * 2),
            error => Task.FromResult(error.Message.Length)
        );
        
        output.ShouldBe(10);
    }

    [Fact]
    public async Task SelectAsync_ChainedOperations_WorksCorrectly() {
        var result = Result<int>.Success(10);
        
        var mapped = await result
            .SelectAsync(x => Task.FromResult(x + 5))
            .SelectAsync(x => x * 2);
        
        mapped.IsSuccess.ShouldBeTrue();
        mapped.Value.ShouldBe(30);
    }

    [Fact]
    public async Task BindAsync_ChainedOperations_WorksCorrectly() {
        var result = Result<int>.Success(10);
        
        var bound = await result
            .BindAsync(x => Task.FromResult(Result<int>.Success(x + 5)))
            .BindAsync(x => Result<string>.Success(x.ToString()));
        
        bound.IsSuccess.ShouldBeTrue();
        bound.Value.ShouldBe("15");
    }

    [Fact]
    public async Task MapErrorAsync_ChainedOperations_WorksCorrectly() {
        var result = Result<int>.Failure(new Error("Error 1"));
        
        var mapped = await result
            .MapErrorAsync(e => Task.FromResult(new Error($"{e.Message} -> Error 2")))
            .MapErrorAsync(e => new Error($"{e.Message} -> Error 3"));
        
        mapped.IsFailure.ShouldBeTrue();
        mapped.Error.Message.ShouldBe("Error 1 -> Error 2 -> Error 3");
    }
}