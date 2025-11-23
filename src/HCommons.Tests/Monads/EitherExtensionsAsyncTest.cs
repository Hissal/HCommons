using HCommons.Monads;

namespace HCommons.Tests.Monads;

[TestSubject(typeof(EitherExtensionsAsync))]
public class EitherExtensionsAsyncTest {

    [Fact]
    public async Task SwapAsync_TaskEither_WhenLeft_ReturnsRight() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var swapped = await eitherTask.SwapAsync();
        swapped.IsRight.ShouldBeTrue();
        swapped.Right.ShouldBe(42);
    }

    [Fact]
    public async Task SwapAsync_TaskEither_WhenRight_ReturnsLeft() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var swapped = await eitherTask.SwapAsync();
        swapped.IsLeft.ShouldBeTrue();
        swapped.Left.ShouldBe("test");
    }

    [Fact]
    public async Task SwapAsync_Either_WhenLeft_ReturnsRight() {
        var either = Either<int, string>.FromLeft(42);
        var swapped = await either.SwapAsync();
        swapped.IsRight.ShouldBeTrue();
        swapped.Right.ShouldBe(42);
    }

    [Fact]
    public async Task SwapAsync_Either_WhenRight_ReturnsLeft() {
        var either = Either<int, string>.FromRight("test");
        var swapped = await either.SwapAsync();
        swapped.IsLeft.ShouldBeTrue();
        swapped.Left.ShouldBe("test");
    }

    [Fact]
    public async Task AsLeftOptionalAsync_TaskEither_WhenLeft_ReturnsSome() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var optional = await eitherTask.AsLeftOptionalAsync();
        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task AsLeftOptionalAsync_TaskEither_WhenRight_ReturnsNone() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var optional = await eitherTask.AsLeftOptionalAsync();
        optional.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task AsLeftOptionalAsync_Either_WhenLeft_ReturnsSome() {
        var either = Either<int, string>.FromLeft(42);
        var optional = await either.AsLeftOptionalAsync();
        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe(42);
    }

    [Fact]
    public async Task AsLeftOptionalAsync_Either_WhenRight_ReturnsNone() {
        var either = Either<int, string>.FromRight("test");
        var optional = await either.AsLeftOptionalAsync();
        optional.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task AsRightOptionalAsync_TaskEither_WhenRight_ReturnsSome() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var optional = await eitherTask.AsRightOptionalAsync();
        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public async Task AsRightOptionalAsync_TaskEither_WhenLeft_ReturnsNone() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var optional = await eitherTask.AsRightOptionalAsync();
        optional.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task AsRightOptionalAsync_Either_WhenRight_ReturnsSome() {
        var either = Either<int, string>.FromRight("test");
        var optional = await either.AsRightOptionalAsync();
        optional.HasValue.ShouldBeTrue();
        optional.Value.ShouldBe("test");
    }

    [Fact]
    public async Task AsRightOptionalAsync_Either_WhenLeft_ReturnsNone() {
        var either = Either<int, string>.FromLeft(42);
        var optional = await either.AsRightOptionalAsync();
        optional.HasValue.ShouldBeFalse();
    }

    [Fact]
    public async Task MapAsync_TaskEitherSyncMappers_WhenLeft_TransformsLeft() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var mapped = await eitherTask.MapAsync(x => x * 2, s => s.Length);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapAsync_TaskEitherSyncMappers_WhenRight_TransformsRight() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var mapped = await eitherTask.MapAsync(x => x * 2, s => s.Length);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapAsync_EitherAsyncLeft_WhenLeft_TransformsLeft() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = await either.MapAsync(x => Task.FromResult(x * 2), s => s.Length);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapAsync_EitherAsyncLeft_WhenRight_DoesNotCallAsyncLeft() {
        var either = Either<int, string>.FromRight("test");
        var mapped = await either.MapAsync(x => Task.FromResult(x * 2), s => s.Length);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapAsync_EitherAsyncRight_WhenRight_TransformsRight() {
        var either = Either<int, string>.FromRight("test");
        var mapped = await either.MapAsync(x => x * 2, s => Task.FromResult(s.Length));
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapAsync_EitherAsyncRight_WhenLeft_DoesNotCallAsyncRight() {
        var either = Either<int, string>.FromLeft(42);
        var asyncCalled = false;
        var mapped = await either.MapAsync(
            x => x * 2,
            async s => {
                asyncCalled = true;
                await Task.CompletedTask;
                return s.Length;
            });
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
        asyncCalled.ShouldBeFalse();
    }

    [Fact]
    public async Task MapAsync_TaskEitherAsyncLeft_WhenLeft_TransformsLeft() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var mapped = await eitherTask.MapAsync(x => Task.FromResult(x * 2), s => s.Length);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapAsync_TaskEitherAsyncRight_WhenRight_TransformsRight() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var mapped = await eitherTask.MapAsync(x => x * 2, s => Task.FromResult(s.Length));
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapAsync_EitherBothAsync_WhenLeft_TransformsLeft() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = await either.MapAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapAsync_EitherBothAsync_WhenRight_TransformsRight() {
        var either = Either<int, string>.FromRight("test");
        var mapped = await either.MapAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapAsync_TaskEitherBothAsync_WhenLeft_TransformsLeft() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var mapped = await eitherTask.MapAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapAsync_TaskEitherBothAsync_WhenRight_TransformsRight() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var mapped = await eitherTask.MapAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapLeftAsync_TaskEither_WhenLeft_TransformsLeft() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var mapped = await eitherTask.MapLeftAsync(x => x * 2);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapLeftAsync_TaskEither_WhenRight_DoesNotTransform() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var mapped = await eitherTask.MapLeftAsync(x => x * 2);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe("test");
    }

    [Fact]
    public async Task MapLeftAsync_EitherAsync_WhenLeft_TransformsLeft() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = await either.MapLeftAsync(x => Task.FromResult(x * 2));
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapLeftAsync_EitherAsync_WhenRight_DoesNotTransform() {
        var either = Either<int, string>.FromRight("test");
        var mapped = await either.MapLeftAsync(x => Task.FromResult(x * 2));
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe("test");
    }

    [Fact]
    public async Task MapLeftAsync_TaskEitherAsync_WhenLeft_TransformsLeft() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var mapped = await eitherTask.MapLeftAsync(x => Task.FromResult(x * 2));
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(84);
    }

    [Fact]
    public async Task MapRightAsync_TaskEither_WhenRight_TransformsRight() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var mapped = await eitherTask.MapRightAsync(s => s.Length);
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapRightAsync_TaskEither_WhenLeft_DoesNotTransform() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var mapped = await eitherTask.MapRightAsync(s => s.Length);
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(42);
    }

    [Fact]
    public async Task MapRightAsync_EitherAsync_WhenRight_TransformsRight() {
        var either = Either<int, string>.FromRight("test");
        var mapped = await either.MapRightAsync(s => Task.FromResult(s.Length));
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MapRightAsync_EitherAsync_WhenLeft_DoesNotTransform() {
        var either = Either<int, string>.FromLeft(42);
        var mapped = await either.MapRightAsync(s => Task.FromResult(s.Length));
        mapped.IsLeft.ShouldBeTrue();
        mapped.Left.ShouldBe(42);
    }

    [Fact]
    public async Task MapRightAsync_TaskEitherAsync_WhenRight_TransformsRight() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var mapped = await eitherTask.MapRightAsync(s => Task.FromResult(s.Length));
        mapped.IsRight.ShouldBeTrue();
        mapped.Right.ShouldBe(4);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherSyncFuncs_WhenLeft_ExecutesLeftFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var result = await eitherTask.MatchAsync(x => x * 2, s => s.Length);
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherSyncFuncs_WhenRight_ExecutesRightFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var result = await eitherTask.MatchAsync(x => x * 2, s => s.Length);
        result.ShouldBe(4);
    }

    [Fact]
    public async Task MatchAsync_EitherAsyncLeft_WhenLeft_ExecutesLeftFunc() {
        var either = Either<int, string>.FromLeft(42);
        var result = await either.MatchAsync(x => Task.FromResult(x * 2), s => s.Length);
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_EitherAsyncLeft_WhenRight_ExecutesRightFunc() {
        var either = Either<int, string>.FromRight("test");
        var result = await either.MatchAsync(x => Task.FromResult(x * 2), s => s.Length);
        result.ShouldBe(4);
    }

    [Fact]
    public async Task MatchAsync_EitherAsyncRight_WhenLeft_ExecutesLeftFunc() {
        var either = Either<int, string>.FromLeft(42);
        var result = await either.MatchAsync(x => x * 2, s => Task.FromResult(s.Length));
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_EitherAsyncRight_WhenRight_ExecutesRightFunc() {
        var either = Either<int, string>.FromRight("test");
        var result = await either.MatchAsync(x => x * 2, s => Task.FromResult(s.Length));
        result.ShouldBe(4);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherAsyncLeft_WhenLeft_ExecutesLeftFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var result = await eitherTask.MatchAsync(x => Task.FromResult(x * 2), s => s.Length);
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherAsyncLeft_WhenRight_ExecutesRightFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var result = await eitherTask.MatchAsync(x => Task.FromResult(x * 2), s => s.Length);
        result.ShouldBe(4);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherAsyncRight_WhenLeft_ExecutesLeftFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var result = await eitherTask.MatchAsync(x => x * 2, s => Task.FromResult(s.Length));
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherAsyncRight_WhenRight_ExecutesRightFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var result = await eitherTask.MatchAsync(x => x * 2, s => Task.FromResult(s.Length));
        result.ShouldBe(4);
    }

    [Fact]
    public async Task MatchAsync_EitherBothAsync_WhenLeft_ExecutesLeftFunc() {
        var either = Either<int, string>.FromLeft(42);
        var result = await either.MatchAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_EitherBothAsync_WhenRight_ExecutesRightFunc() {
        var either = Either<int, string>.FromRight("test");
        var result = await either.MatchAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        result.ShouldBe(4);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherBothAsync_WhenLeft_ExecutesLeftFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromLeft(42));
        var result = await eitherTask.MatchAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        result.ShouldBe(84);
    }

    [Fact]
    public async Task MatchAsync_TaskEitherBothAsync_WhenRight_ExecutesRightFunc() {
        var eitherTask = Task.FromResult(Either<int, string>.FromRight("test"));
        var result = await eitherTask.MatchAsync(x => Task.FromResult(x * 2), s => Task.FromResult(s.Length));
        result.ShouldBe(4);
    }

    [Fact]
    public async Task MapAsync_ChainedOperations_WorksCorrectly() {
        var either = Either<int, string>.FromLeft(10);
        var result = await either
            .MapLeftAsync(x => Task.FromResult(x * 2))
            .MapLeftAsync(x => x + 5);
        result.IsLeft.ShouldBeTrue();
        result.Left.ShouldBe(25);
    }

    [Fact]
    public async Task SwapAsync_TwiceReturnsOriginal() {
        var either = Either<int, string>.FromLeft(42);
        var swapped = await either.SwapAsync();
        var original = await swapped.SwapAsync();
        original.IsLeft.ShouldBeTrue();
        original.Left.ShouldBe(42);
    }
}

