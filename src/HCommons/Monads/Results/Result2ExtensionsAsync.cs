using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for Result{TSuccess, TFailure} types.
/// </summary>
public static class Result2ExtensionsAsync {
    /// <summary>
    /// Matches on the result, applying one of two functions depending on success or failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return (await resultTask).Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, TResult> onFailure)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value!) : onFailure(result.FailureValue!);
    }

    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value!) : await onFailureAsync(result.FailureValue!);
    }

    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, TResult> onFailure)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure);
    }

    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync);
    }

    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value!) : await onFailureAsync(result.FailureValue!);
    }

    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync);
    }
}
