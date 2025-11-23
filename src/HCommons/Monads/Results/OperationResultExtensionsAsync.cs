using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for OperationResult types.
/// </summary>
public static class OperationResultExtensionsAsync {
    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <param name="resultTask">The task containing the operation result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult> MapErrorAsync(this Task<OperationResult> resultTask, Func<Error, Error> mapError) {
        return (await resultTask).MapError(mapError);
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult> MapErrorAsync(this OperationResult result, Func<Error, Task<Error>> mapErrorAsync) {
        return result.IsFailure ? OperationResult.Failure(await mapErrorAsync(result.Error)) : result;
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <param name="resultTask">The task containing the operation result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult> MapErrorAsync(this Task<OperationResult> resultTask, Func<Error, Task<Error>> mapErrorAsync) {
        return await (await resultTask).MapErrorAsync(mapErrorAsync);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <param name="resultTask">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult> MapCancellationAsync(this Task<OperationResult> resultTask, Func<Cancelled, Cancelled> mapCancellation) {
        return (await resultTask).MapCancellation(mapCancellation);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="mapCancellationAsync">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult> MapCancellationAsync(this OperationResult result, Func<Cancelled, Task<Cancelled>> mapCancellationAsync) {
        return result.IsCancelled ? OperationResult.Cancelled(await mapCancellationAsync(result.Cancellation)) : result;
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <param name="resultTask">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellationAsync">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult> MapCancellationAsync(this Task<OperationResult> resultTask, Func<Cancelled, Task<Cancelled>> mapCancellationAsync) {
        return await (await resultTask).MapCancellationAsync(mapCancellationAsync);
    }

    /// <summary>
    /// Matches on the operation result, applying one of three functions depending on success, failure, or cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> resultTask,
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return (await resultTask).Match(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return result.IsSuccess ? await onSuccessAsync() : result.IsFailure ? onFailure(result.Error) : onCancelled(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync,
        Func<Cancelled, TResult> onCancelled)
    {
        return result.IsSuccess ? onSuccess() : result.IsFailure ? await onFailureAsync(result.Error) : onCancelled(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return result.IsSuccess ? onSuccess() : result.IsFailure ? onFailure(result.Error) : await onCancelledAsync(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync,
        Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return result.IsSuccess ? await onSuccessAsync() : result.IsFailure ? await onFailureAsync(result.Error) : await onCancelledAsync(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> resultTask,
        Func<Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync,
        Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync, onCancelledAsync);
    }

    #region Additional Match Overloads

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this OperationResult result, Func<Task<TResult>> onSuccessAsync, Func<Error, Task<TResult>> onFailureAsync, Func<Cancelled, TResult> onCancelled)
    {
        if (result.IsSuccess) return await onSuccessAsync();
        if (result.IsFailure) return await onFailureAsync(result.Error);
        return onCancelled(result.Cancellation);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this OperationResult result, Func<Task<TResult>> onSuccessAsync, Func<Error, TResult> onFailure, Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        if (result.IsSuccess) return await onSuccessAsync();
        if (result.IsFailure) return onFailure(result.Error);
        return await onCancelledAsync(result.Cancellation);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this OperationResult result, Func<TResult> onSuccess, Func<Error, Task<TResult>> onFailureAsync, Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        if (result.IsSuccess) return onSuccess();
        if (result.IsFailure) return await onFailureAsync(result.Error);
        return await onCancelledAsync(result.Cancellation);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this Task<OperationResult> resultTask, Func<Task<TResult>> onSuccessAsync, Func<Error, TResult> onFailure, Func<Cancelled, TResult> onCancelled)
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure, onCancelled);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this Task<OperationResult> resultTask, Func<TResult> onSuccess, Func<Error, Task<TResult>> onFailureAsync, Func<Cancelled, TResult> onCancelled)
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync, onCancelled);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this Task<OperationResult> resultTask, Func<TResult> onSuccess, Func<Error, TResult> onFailure, Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailure, onCancelledAsync);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this Task<OperationResult> resultTask, Func<Task<TResult>> onSuccessAsync, Func<Error, Task<TResult>> onFailureAsync, Func<Cancelled, TResult> onCancelled)
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync, onCancelled);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this Task<OperationResult> resultTask, Func<Task<TResult>> onSuccessAsync, Func<Error, TResult> onFailure, Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure, onCancelledAsync);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(this Task<OperationResult> resultTask, Func<TResult> onSuccess, Func<Error, Task<TResult>> onFailureAsync, Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync, onCancelledAsync);
    }

    #endregion
}
