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
    /// <param name="result">The task containing the operation result to transform.</param>
    /// <param name="selector">The function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> SelectAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TSuccess, TResult> selector)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return (await result).Select(selector);
    }

    /// <summary>
    /// Maps an operation result success value to a new value using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="selector">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> SelectAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, Task<TResult>> selector)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => OperationResult<TResult, TFailure, TCancelled>.Success(await selector(result.SuccessValue!)),
            OperationResultType.Failure => OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Maps an operation result success value to a new value using the specified asynchronous selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The task containing the operation result to transform.</param>
    /// <param name="selector">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> SelectAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TSuccess, Task<TResult>> selector)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return await (await result).SelectAsync(selector);
    }

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The task containing the operation result to bind.</param>
    /// <param name="binder">The function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> BindAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TSuccess, OperationResult<TResult, TFailure, TCancelled>> binder)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return (await result).Bind(binder);
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="binder">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> BindAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, Task<OperationResult<TResult, TFailure, TCancelled>>> binder)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => await binder(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The task containing the operation result to bind.</param>
    /// <param name="binder">The asynchronous function to apply to the success value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult, TFailure, TCancelled>> BindAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TSuccess, Task<OperationResult<TResult, TFailure, TCancelled>>> binder)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull 
    {
        return await (await result).BindAsync(binder);
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
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TFailure, TNewFailure> mapFailure)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull 
    {
        return (await result).MapError(mapFailure);
    }

    /// <summary>
    /// Transforms the failure value of a failed operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The operation result whose failure value to transform.</param>
    /// <param name="mapFailure">The asynchronous function to apply to the failure value if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed failure value if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TNewFailure, TCancelled>> MapErrorAsync<TSuccess, TFailure, TCancelled, TNewFailure>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TFailure, Task<TNewFailure>> mapFailure)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull {
        return result.Type switch {
            OperationResultType.Success => OperationResult<TSuccess, TNewFailure, TCancelled>.Success(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TSuccess, TNewFailure, TCancelled>.Failure(await mapFailure(result.FailureValue!)),
            OperationResultType.Cancelled => OperationResult<TSuccess, TNewFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Transforms the failure value of a failed operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The task containing the operation result whose failure value to transform.</param>
    /// <param name="mapFailure">The asynchronous function to apply to the failure value if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed failure value if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TNewFailure, TCancelled>> MapErrorAsync<TSuccess, TFailure, TCancelled, TNewFailure>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TFailure, Task<TNewFailure>> mapFailure)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull 
    {
        return await (await result).MapErrorAsync(mapFailure);
    }

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="result">The task containing the operation result whose cancellation value to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation value if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TFailure, TNewCancelled>> MapCancellationAsync<TSuccess, TFailure, TCancelled, TNewCancelled>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TCancelled, TNewCancelled> mapCancellation)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull 
    {
        return (await result).MapCancellation(mapCancellation);
    }

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="result">The operation result whose cancellation value to transform.</param>
    /// <param name="mapCancellation">The asynchronous function to apply to the cancellation value if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TFailure, TNewCancelled>> MapCancellationAsync<TSuccess, TFailure, TCancelled, TNewCancelled>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TCancelled, Task<TNewCancelled>> mapCancellation)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => OperationResult<TSuccess, TFailure, TNewCancelled>.Success(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TSuccess, TFailure, TNewCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TSuccess, TFailure, TNewCancelled>.Cancelled(await mapCancellation(result.CancelledValue!)),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="result">The task containing the operation result whose cancellation value to transform.</param>
    /// <param name="mapCancellation">The asynchronous function to apply to the cancellation value if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TSuccess, TFailure, TNewCancelled>> MapCancellationAsync<TSuccess, TFailure, TCancelled, TNewCancelled>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TCancelled, Task<TNewCancelled>> mapCancellation)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull 
    {
        return await (await result).MapCancellationAsync(mapCancellation);
    }

    #region MatchAsync
    
    /// <summary>
    /// Matches on the operation result, applying one of three functions depending on success, failure, or cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return (await result).Match(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, Task<TResult>> onSuccess,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.SuccessValue!),
            OperationResultType.Failure => onFailure(result.FailureValue!),
            OperationResultType.Cancelled => onCancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
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
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.SuccessValue!),
            OperationResultType.Failure => await onFailure(result.FailureValue!),
            OperationResultType.Cancelled => onCancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
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
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.SuccessValue!),
            OperationResultType.Failure => onFailure(result.FailureValue!),
            OperationResultType.Cancelled => await onCancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }
    
    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result,
        Func<TSuccess, Task<TResult>> onSuccess,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, Task<TResult>> onSuccess, 
        Func<TFailure, Task<TResult>> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.SuccessValue!),
            OperationResultType.Failure => await onFailure(result.FailureValue!),
            OperationResultType.Cancelled => onCancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and cancellation or synchronous function on failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, Task<TResult>> onSuccess, 
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.SuccessValue!),
            OperationResultType.Failure => onFailure(result.FailureValue!),
            OperationResultType.Cancelled => await onCancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or asynchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, TResult> onSuccess, 
        Func<TFailure, Task<TResult>> onFailure, 
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.SuccessValue!),
            OperationResultType.Failure => await onFailure(result.FailureValue!),
            OperationResultType.Cancelled => await onCancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and failure or synchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result,
        Func<TSuccess, Task<TResult>> onSuccess,
        Func<TFailure, Task<TResult>> onFailure,
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and cancellation or synchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TSuccess, Task<TResult>> onSuccess, 
        Func<TFailure, TResult> onFailure,
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or asynchronous functions on failure and cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result, 
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailure, 
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, Task<TResult>> onSuccess,
        Func<TFailure, Task<TResult>> onFailure,
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.SuccessValue!),
            OperationResultType.Failure => await onFailure(result.FailureValue!),
            OperationResultType.Cancelled => await onCancelled(result.CancelledValue!),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };  
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TCancelled, TResult>(
        this Task<OperationResult<TSuccess, TFailure, TCancelled>> result,
        Func<TSuccess, Task<TResult>> onSuccess,
        Func<TFailure, Task<TResult>> onFailure,
        Func<TCancelled, Task<TResult>> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    #endregion
    
}
