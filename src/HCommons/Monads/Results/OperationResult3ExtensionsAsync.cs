using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for OperationResult&lt;TSuccess, TFailure, TCancelled&gt; types.
/// </summary>
public static class OperationResult3ExtensionsAsync {
    /// <summary>
    /// Maps an operation result success value to a new value using the specified selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to transform.</param>
    /// <param name="selector">The function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> SelectAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TSuccess, TResult> selector)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return (await resultTask).Select(selector);
    }

    /// <summary>
    /// Maps an operation result success value to a new value using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> SelectAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, Task<TResult>> selectorAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return result.IsSuccess 
            ? OperationResult<TResult, TFailure, TCancelled>.Success(await selectorAsync(result.SuccessValue!)) 
            : result.IsFailure 
                ? OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!) 
                : OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!);
    }

    /// <summary>
    /// Maps an operation result success value to a new value using the specified asynchronous selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> SelectAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TSuccess, Task<TResult>> selectorAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return await (await resultTask).SelectAsync(selectorAsync);
    }

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to bind.</param>
    /// <param name="binder">The function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> BindAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TSuccess, OperationResult<TResult, TFailure, TCancelled>> binder)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return (await resultTask).Bind(binder);
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> BindAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, Task<OperationResult<TResult, TFailure, TCancelled>>> binderAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return result.IsSuccess 
            ? await binderAsync(result.SuccessValue!) 
            : result.IsFailure 
                ? OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!) 
                : OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!);
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> BindAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TSuccess, Task<OperationResult<TResult, TFailure, TCancelled>>> binderAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return await (await resultTask).BindAsync(binderAsync);
    }

    /// <summary>
    /// Transforms the failure value of a failed operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose failure value to transform.</param>
    /// <param name="mapFailure">The function to apply to the failure value if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed failure value if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TNewFailure, TCancelled>> MapErrorAsync<TSuccess, TFailure, TCancelled, TNewFailure>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TFailure, TNewFailure> mapFailure)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull 
    {
        return (await resultTask).MapError(mapFailure);
    }

    /// <summary>
    /// Transforms the failure value of a failed operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The operation result whose failure value to transform.</param>
    /// <param name="mapFailureAsync">The asynchronous function to apply to the failure value if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed failure value if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TNewFailure, TCancelled>> MapErrorAsync<TSuccess, TFailure, TCancelled, TNewFailure>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TFailure, Task<TNewFailure>> mapFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull 
    {
        return result.IsFailure 
            ? OperationResult<TSuccess, TNewFailure, TCancelled>.Failure(await mapFailureAsync(result.FailureValue!)) 
            : result.IsSuccess
                ? OperationResult<TSuccess, TNewFailure, TCancelled>.Success(result.SuccessValue!)
                : OperationResult<TSuccess, TNewFailure, TCancelled>.Cancelled(result.CancelledValue!);
    }

    /// <summary>
    /// Transforms the failure value of a failed operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose failure value to transform.</param>
    /// <param name="mapFailureAsync">The asynchronous function to apply to the failure value if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed failure value if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TNewFailure, TCancelled>> MapErrorAsync<TSuccess, TFailure, TCancelled, TNewFailure>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TFailure, Task<TNewFailure>> mapFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull 
    {
        return await (await resultTask).MapErrorAsync(mapFailureAsync);
    }

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose cancellation value to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation value if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TFailure, TNewCancelled>> MapCancellationAsync<TSuccess, TFailure, TCancelled, TNewCancelled>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TCancelled, TNewCancelled> mapCancellation)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull 
    {
        return (await resultTask).MapCancellation(mapCancellation);
    }

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="result">The operation result whose cancellation value to transform.</param>
    /// <param name="mapCancellationAsync">The asynchronous function to apply to the cancellation value if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TFailure, TNewCancelled>> MapCancellationAsync<TSuccess, TFailure, TCancelled, TNewCancelled>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TCancelled, Task<TNewCancelled>> mapCancellationAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull 
    {
        return result.IsCancelled 
            ? OperationResult<TSuccess, TFailure, TNewCancelled>.Cancelled(await mapCancellationAsync(result.CancelledValue!)) 
            : result.IsSuccess
                ? OperationResult<TSuccess, TFailure, TNewCancelled>.Success(result.SuccessValue!)
                : OperationResult<TSuccess, TFailure, TNewCancelled>.Failure(result.FailureValue!);
    }

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose cancellation value to transform.</param>
    /// <param name="mapCancellationAsync">The asynchronous function to apply to the cancellation value if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TFailure, TNewCancelled>> MapCancellationAsync<TSuccess, TFailure, TCancelled, TNewCancelled>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, 
        Func<TCancelled, Task<TNewCancelled>> mapCancellationAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull 
    {
        return await (await resultTask).MapCancellationAsync(mapCancellationAsync);
    }

    /// <summary>
    /// Matches on the operation result, applying one of three functions depending on success, failure, or cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return (await resultTask).Match(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.SuccessValue!) : result.IsFailure ? onFailure(result.FailureValue!) : onCancelled(result.CancelledValue!);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailureAsync,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.IsSuccess ? onSuccess(result.SuccessValue!) : result.IsFailure ? await onFailureAsync(result.FailureValue!) : onCancelled(result.CancelledValue!);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, Task<TResult>> onCancelledAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.IsSuccess ? onSuccess(result.SuccessValue!) : result.IsFailure ? onFailure(result.FailureValue!) : await onCancelledAsync(result.CancelledValue!);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, Task<TResult>> onFailureAsync,
        Func<TCancelled, Task<TResult>> onCancelledAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.SuccessValue!) : result.IsFailure ? await onFailureAsync(result.FailureValue!) : await onCancelledAsync(result.CancelledValue!);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, Task<TResult>> onFailureAsync,
        Func<TCancelled, Task<TResult>> onCancelledAsync)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync, onCancelledAsync);
    }

    #region Additional Match Overloads

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this OperationResult<TSuccess, TFailure, TCancelled> result, Func<TSuccess, Task<TResult>> onSuccessAsync, Func<TFailure, Task<TResult>> onFailureAsync, Func<TCancelled, TResult> onCancelled) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        if (result.IsSuccess) return await onSuccessAsync(result.SuccessValue!);
        if (result.IsFailure) return await onFailureAsync(result.FailureValue!);
        return onCancelled(result.CancelledValue!);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this OperationResult<TSuccess, TFailure, TCancelled> result, Func<TSuccess, Task<TResult>> onSuccessAsync, Func<TFailure, TResult> onFailure, Func<TCancelled, Task<TResult>> onCancelledAsync) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        if (result.IsSuccess) return await onSuccessAsync(result.SuccessValue!);
        if (result.IsFailure) return onFailure(result.FailureValue!);
        return await onCancelledAsync(result.CancelledValue!);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this OperationResult<TSuccess, TFailure, TCancelled> result, Func<TSuccess, TResult> onSuccess, Func<TFailure, Task<TResult>> onFailureAsync, Func<TCancelled, Task<TResult>> onCancelledAsync) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        if (result.IsSuccess) return onSuccess(result.SuccessValue!);
        if (result.IsFailure) return await onFailureAsync(result.FailureValue!);
        return await onCancelledAsync(result.CancelledValue!);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, Func<TSuccess, Task<TResult>> onSuccessAsync, Func<TFailure, TResult> onFailure, Func<TCancelled, TResult> onCancelled) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure, onCancelled);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, Func<TSuccess, TResult> onSuccess, Func<TFailure, Task<TResult>> onFailureAsync, Func<TCancelled, TResult> onCancelled) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync, onCancelled);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, Func<TSuccess, TResult> onSuccess, Func<TFailure, TResult> onFailure, Func<TCancelled, Task<TResult>> onCancelledAsync) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailure, onCancelledAsync);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, Func<TSuccess, Task<TResult>> onSuccessAsync, Func<TFailure, Task<TResult>> onFailureAsync, Func<TCancelled, TResult> onCancelled) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync, onCancelled);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, Func<TSuccess, Task<TResult>> onSuccessAsync, Func<TFailure, TResult> onFailure, Func<TCancelled, Task<TResult>> onCancelledAsync) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure, onCancelledAsync);
    }

    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(this Task<OperationResult<TSuccess, TFailure, TCancelled>> resultTask, Func<TSuccess, TResult> onSuccess, Func<TFailure, Task<TResult>> onFailureAsync, Func<TCancelled, Task<TResult>> onCancelledAsync) where TSuccess : notnull where TFailure : notnull where TCancelled : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync, onCancelledAsync);
    }

    #endregion
}
