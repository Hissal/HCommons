using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for OperationResult&lt;TValue&gt; types.
/// </summary>
public static class OperationResult1ExtensionsAsync {
    /// <summary>
    /// Maps an operation result value to a new value using the specified selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to transform.</param>
    /// <param name="selector">The function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> SelectAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<TValue, TResult> selector) 
    {
        return (await resultTask).Select(selector);
    }

    /// <summary>
    /// Maps an operation result value to a new value using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> SelectAsync<TValue, TResult>(
        this OperationResult<TValue> result, 
        Func<TValue, Task<TResult>> selectorAsync) 
    {
        return result.IsSuccess 
            ? OperationResult<TResult>.Success(await selectorAsync(result.Value!)) 
            : result.IsFailure 
                ? OperationResult<TResult>.Failure(result.Error) 
                : OperationResult<TResult>.Cancelled(result.Cancellation);
    }

    /// <summary>
    /// Maps an operation result value to a new value using the specified asynchronous selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> SelectAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<TValue, Task<TResult>> selectorAsync) 
    {
        return await (await resultTask).SelectAsync(selectorAsync);
    }

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to bind.</param>
    /// <param name="binder">The function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> BindAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<TValue, OperationResult<TResult>> binder) 
    {
        return (await resultTask).Bind(binder);
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> BindAsync<TValue, TResult>(
        this OperationResult<TValue> result, 
        Func<TValue, Task<OperationResult<TResult>>> binderAsync) 
    {
        return result.IsSuccess 
            ? await binderAsync(result.Value!) 
            : result.IsFailure 
                ? OperationResult<TResult>.Failure(result.Error) 
                : OperationResult<TResult>.Cancelled(result.Cancellation);
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the operation result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> BindAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<TValue, Task<OperationResult<TResult>>> binderAsync) 
    {
        return await (await resultTask).BindAsync(binderAsync);
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapErrorAsync<TValue>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<Error, Error> mapError) 
    {
        return (await resultTask).MapError(mapError);
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapErrorAsync<TValue>(
        this OperationResult<TValue> result, 
        Func<Error, Task<Error>> mapErrorAsync) 
    {
        return result.IsFailure ? OperationResult<TValue>.Failure(await mapErrorAsync(result.Error)) : result;
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapErrorAsync<TValue>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<Error, Task<Error>> mapErrorAsync) 
    {
        return await (await resultTask).MapErrorAsync(mapErrorAsync);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapCancellationAsync<TValue>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<Cancelled, Cancelled> mapCancellation) 
    {
        return (await resultTask).MapCancellation(mapCancellation);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="mapCancellationAsync">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapCancellationAsync<TValue>(
        this OperationResult<TValue> result, 
        Func<Cancelled, Task<Cancelled>> mapCancellationAsync) 
    {
        return result.IsCancelled ? OperationResult<TValue>.Cancelled(await mapCancellationAsync(result.Cancellation)) : result;
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="resultTask">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellationAsync">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapCancellationAsync<TValue>(
        this Task<OperationResult<TValue>> resultTask, 
        Func<Cancelled, Task<Cancelled>> mapCancellationAsync) 
    {
        return await (await resultTask).MapCancellationAsync(mapCancellationAsync);
    }

    /// <summary>
    /// Matches on the operation result, applying one of three functions depending on success, failure, or cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return (await resultTask).Match(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value!) : result.IsFailure ? onFailure(result.Error) : onCancelled(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync,
        Func<Cancelled, TResult> onCancelled)
    {
        return result.IsSuccess ? onSuccess(result.Value!) : result.IsFailure ? await onFailureAsync(result.Error) : onCancelled(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return result.IsSuccess ? onSuccess(result.Value!) : result.IsFailure ? onFailure(result.Error) : await onCancelledAsync(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync,
        Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value!) : result.IsFailure ? await onFailureAsync(result.Error) : await onCancelledAsync(result.Cancellation);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the operation result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelledAsync">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync,
        Func<Cancelled, Task<TResult>> onCancelledAsync)
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync, onCancelledAsync);
    }
}
