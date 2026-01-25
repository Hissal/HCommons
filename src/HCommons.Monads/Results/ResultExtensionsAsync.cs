using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for Result types.
/// </summary>
public static class ResultExtensionsAsync {
    /// <summary>
    /// Transforms the error of a failed result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <param name="resultTask">The task containing the result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the result is failed.</param>
    /// <returns>A task containing a result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static async Task<Result> MapErrorAsync(this Task<Result> resultTask, Func<Error, Error> mapError) {
        return (await resultTask).MapError(mapError);
    }

    /// <summary>
    /// Transforms the error of a failed result using the specified asynchronous mapping function.
    /// </summary>
    /// <param name="result">The result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the result is failed.</param>
    /// <returns>A task containing a result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static async Task<Result> MapErrorAsync(this Result result, Func<Error, Task<Error>> mapErrorAsync) {
        return result.IsFailure ? Result.Failure(await mapErrorAsync(result.Error)) : result;
    }

    /// <summary>
    /// Transforms the error of a failed result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <param name="resultTask">The task containing the result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the result is failed.</param>
    /// <returns>A task containing a result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static async Task<Result> MapErrorAsync(this Task<Result> resultTask, Func<Error, Task<Error>> mapErrorAsync) {
        return await (await resultTask).MapErrorAsync(mapErrorAsync);
    }

    /// <summary>
    /// Matches on the result, applying one of two functions depending on success or failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<Result> resultTask,
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return (await resultTask).Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Result result,
        Func<Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure)
    {
        return result.IsSuccess ? await onSuccessAsync() : onFailure(result.Error);
    }

    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Result result,
        Func<TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync)
    {
        return result.IsSuccess ? onSuccess() : await onFailureAsync(result.Error);
    }

    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<Result> resultTask,
        Func<Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure)
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure);
    }

    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<Result> resultTask,
        Func<TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync)
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync);
    }

    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Result result,
        Func<Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync)
    {
        return result.IsSuccess ? await onSuccessAsync() : await onFailureAsync(result.Error);
    }

    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<Result> resultTask,
        Func<Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync)
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync);
    }
}
