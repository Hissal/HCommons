using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(OptionalExtensionsAsync))]
public class OptionalExtensionsAsyncTest {

    [Fact]
    public async Task SelectAsync_TaskOptional_WhenHasValue_TransformsValue() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var mapped = await optionalTask.SelectAsync(x => x * 2);
        
        mapped.HasValue.ShouldBeTrue();
        mapped.Value.ShouldBe(84);
    }

    [Fact]
    public async Task SelectAsync_TaskOptional_WhenNone_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var mapped = await optionalTask.SelectAsync(x => x * 2);
        
        mapped.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_WhenHasValue_TransformsValue() {
        var optional = Optional<int>.Some(42);
        
        var mapped = await optional.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.HasValue.ShouldBeTrue();
        mapped.Value.ShouldBe(84);
    }

    [Fact]
    public async Task SelectAsync_AsyncSelector_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var mapped = await optional.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task SelectAsync_TaskOptionalAsyncSelector_WhenHasValue_TransformsValue() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var mapped = await optionalTask.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.HasValue.ShouldBeTrue();
        mapped.Value.ShouldBe(84);
    }

    [Fact]
    public async Task SelectAsync_TaskOptionalAsyncSelector_WhenNone_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var mapped = await optionalTask.SelectAsync(x => Task.FromResult(x * 2));
        
        mapped.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task BindAsync_TaskOptional_WhenHasValue_ExecutesBinder() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var bound = await optionalTask.BindAsync(x => Optional<string>.Some(x.ToString()));
        
        bound.HasValue.ShouldBeTrue();
        bound.Value.ShouldBe("42");
    }

    [Fact]
    public async Task BindAsync_TaskOptional_WhenHasValue_BinderReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var bound = await optionalTask.BindAsync(_ => Optional<string>.None());
        
        bound.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task BindAsync_TaskOptional_WhenNone_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var bound = await optionalTask.BindAsync(x => Optional<string>.Some(x.ToString()));
        
        bound.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_WhenHasValue_ExecutesBinder() {
        var optional = Optional<int>.Some(42);
        
        var bound = await optional.BindAsync(x => Task.FromResult(Optional<string>.Some(x.ToString())));
        
        bound.HasValue.ShouldBeTrue();
        bound.Value.ShouldBe("42");
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_WhenHasValue_BinderReturnsNone() {
        var optional = Optional<int>.Some(42);
        
        var bound = await optional.BindAsync(_ => Task.FromResult(Optional<string>.None()));
        
        bound.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task BindAsync_AsyncBinder_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var bound = await optional.BindAsync(x => Task.FromResult(Optional<string>.Some(x.ToString())));
        
        bound.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task BindAsync_TaskOptionalAsyncBinder_WhenHasValue_ExecutesBinder() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var bound = await optionalTask.BindAsync(x => Task.FromResult(Optional<string>.Some(x.ToString())));
        
        bound.HasValue.ShouldBeTrue();
        bound.Value.ShouldBe("42");
    }

    [Fact]
    public async Task BindAsync_TaskOptionalAsyncBinder_WhenNone_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var bound = await optionalTask.BindAsync(x => Task.FromResult(Optional<string>.Some(x.ToString())));
        
        bound.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task WhereAsync_TaskOptional_WhenHasValue_AndPredicateIsTrue_ReturnsSameOptional() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var filtered = await optionalTask.WhereAsync(x => x > 40);
        
        filtered.HasValue.ShouldBeTrue();
        filtered.Value.ShouldBe(42);
    }

    [Fact]
    public async Task WhereAsync_TaskOptional_WhenHasValue_AndPredicateIsFalse_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var filtered = await optionalTask.WhereAsync(x => x > 50);
        
        filtered.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task WhereAsync_TaskOptional_WhenNone_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var filtered = await optionalTask.WhereAsync(x => x > 40);
        
        filtered.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task WhereAsync_AsyncPredicate_WhenHasValue_AndPredicateIsTrue_ReturnsSameOptional() {
        var optional = Optional<int>.Some(42);
        
        var filtered = await optional.WhereAsync(x => Task.FromResult(x > 40));
        
        filtered.HasValue.ShouldBeTrue();
        filtered.Value.ShouldBe(42);
    }

    [Fact]
    public async Task WhereAsync_AsyncPredicate_WhenHasValue_AndPredicateIsFalse_ReturnsNone() {
        var optional = Optional<int>.Some(42);
        
        var filtered = await optional.WhereAsync(x => Task.FromResult(x > 50));
        
        filtered.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task WhereAsync_AsyncPredicate_WhenNone_ReturnsNone() {
        var optional = Optional<int>.None();
        
        var filtered = await optional.WhereAsync(x => Task.FromResult(x > 40));
        
        filtered.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task WhereAsync_TaskOptionalAsyncPredicate_WhenHasValue_AndPredicateIsTrue_ReturnsSameOptional() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var filtered = await optionalTask.WhereAsync(x => Task.FromResult(x > 40));
        
        filtered.HasValue.ShouldBeTrue();
        filtered.Value.ShouldBe(42);
    }

    [Fact]
    public async Task WhereAsync_TaskOptionalAsyncPredicate_WhenHasValue_AndPredicateIsFalse_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var filtered = await optionalTask.WhereAsync(x => Task.FromResult(x > 50));
        
        filtered.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task WhereAsync_TaskOptionalAsyncPredicate_WhenNone_ReturnsNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var filtered = await optionalTask.WhereAsync(x => Task.FromResult(x > 40));
        
        filtered.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task MatchAsync_TaskOptional_WhenHasValue_ExecutesOnValue() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var output = await optionalTask.MatchAsync(
            value => value * 2,
            () => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskOptional_WhenNone_ExecutesOnNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var output = await optionalTask.MatchAsync(
            value => value * 2,
            () => 0
        );
        
        output.ShouldBe(0);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnValue_WhenHasValue_ExecutesOnValue() {
        var optional = Optional<int>.Some(42);
        
        var output = await optional.MatchAsync(
            value => Task.FromResult(value * 2),
            () => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnValue_WhenNone_ExecutesOnNone() {
        var optional = Optional<int>.None();
        
        var output = await optional.MatchAsync(
            value => Task.FromResult(value * 2),
            () => 0
        );
        
        output.ShouldBe(0);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnNone_WhenHasValue_ExecutesOnValue() {
        var optional = Optional<int>.Some(42);
        
        var output = await optional.MatchAsync(
            value => value * 2,
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_AsyncOnNone_WhenNone_ExecutesOnNone() {
        var optional = Optional<int>.None();
        
        var output = await optional.MatchAsync(
            value => value * 2,
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(0);
    }

    [Fact]
    public async Task MatchAsync_TaskOptionalAsyncOnValue_WhenHasValue_ExecutesOnValue() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var output = await optionalTask.MatchAsync(
            value => Task.FromResult(value * 2),
            () => 0
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskOptionalAsyncOnValue_WhenNone_ExecutesOnNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var output = await optionalTask.MatchAsync(
            value => Task.FromResult(value * 2),
            () => 0
        );
        
        output.ShouldBe(0);
    }

    [Fact]
    public async Task MatchAsync_TaskOptionalAsyncOnNone_WhenHasValue_ExecutesOnValue() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var output = await optionalTask.MatchAsync(
            value => value * 2,
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskOptionalAsyncOnNone_WhenNone_ExecutesOnNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var output = await optionalTask.MatchAsync(
            value => value * 2,
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(0);
    }

    [Fact]
    public async Task MatchAsync_BothAsync_WhenHasValue_ExecutesOnValue() {
        var optional = Optional<int>.Some(42);
        
        var output = await optional.MatchAsync(
            value => Task.FromResult(value * 2),
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_BothAsync_WhenNone_ExecutesOnNone() {
        var optional = Optional<int>.None();
        
        var output = await optional.MatchAsync(
            value => Task.FromResult(value * 2),
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(0);
    }

    [Fact]
    public async Task MatchAsync_TaskOptionalBothAsync_WhenHasValue_ExecutesOnValue() {
        var optionalTask = Task.FromResult(Optional<int>.Some(42));
        
        var output = await optionalTask.MatchAsync(
            value => Task.FromResult(value * 2),
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskOptionalBothAsync_WhenNone_ExecutesOnNone() {
        var optionalTask = Task.FromResult(Optional<int>.None());
        
        var output = await optionalTask.MatchAsync(
            value => Task.FromResult(value * 2),
            () => Task.FromResult(0)
        );
        
        output.ShouldBe(0);
    }

    [Fact]
    public async Task SelectAsync_ChainedOperations_WorksCorrectly() {
        var optional = Optional<int>.Some(10);
        
        var mapped = await optional
            .SelectAsync(x => Task.FromResult(x + 5))
            .SelectAsync(x => x * 2);
        
        mapped.HasValue.ShouldBeTrue();
        mapped.Value.ShouldBe(30);
    }

    [Fact]
    public async Task BindAsync_ChainedOperations_WorksCorrectly() {
        var optional = Optional<int>.Some(10);
        
        var bound = await optional
            .BindAsync(x => Task.FromResult(Optional<int>.Some(x + 5)))
            .BindAsync(x => Optional<string>.Some(x.ToString()));
        
        bound.HasValue.ShouldBeTrue();
        bound.Value.ShouldBe("15");
    }

    [Fact]
    public async Task WhereAsync_ChainedOperations_WorksCorrectly() {
        var optional = Optional<int>.Some(42);
        
        var filtered = await optional
            .WhereAsync(x => Task.FromResult(x > 40))
            .WhereAsync(x => x < 50);
        
        filtered.HasValue.ShouldBeTrue();
        filtered.Value.ShouldBe(42);
    }

    [Fact]
    public async Task WhereAsync_ChainedOperations_FiltersFails_ReturnsNone() {
        var optional = Optional<int>.Some(42);
        
        var filtered = await optional
            .WhereAsync(x => Task.FromResult(x > 40))
            .WhereAsync(x => x > 50);
        
        filtered.HasValue.ShouldBeFalse();
    }
}
