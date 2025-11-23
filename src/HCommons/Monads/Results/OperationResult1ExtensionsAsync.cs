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
    /// <param name="result">The task containing the operation result to transform.</param>
    /// <param name="selector">The function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> SelectAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result, 
        Func<TValue, TResult> selector) 
        where TValue : notnull
        where TResult : notnull
    {
        return (await result).Select(selector);
    }

    /// <summary>
    /// Maps an operation result value to a new value using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="selector">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> SelectAsync<TValue, TResult>(
        this OperationResult<TValue> result, 
        Func<TValue, Task<TResult>> selector) 
        where TValue : notnull
        where TResult : notnull
    {
        return result.Type switch {
            OperationResultType.Success => OperationResult<TResult>.Success(await selector(result.Value!)),
            OperationResultType.Failure => OperationResult<TResult>.Failure(result.Error),
            OperationResultType.Cancelled => OperationResult<TResult>.Cancelled(result.Cancellation),
                        _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Maps an operation result value to a new value using the specified asynchronous selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The task containing the operation result to transform.</param>
    /// <param name="selector">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing an operation result with the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> SelectAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result, 
        Func<TValue, Task<TResult>> selector) 
        where TValue : notnull
        where TResult : notnull
    {
        return await (await result).SelectAsync(selector);
    }

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The task containing the operation result to bind.</param>
    /// <param name="binder">The function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> BindAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result, 
        Func<TValue, OperationResult<TResult>> binder) 
        where TValue : notnull
        where TResult : notnull
    {
        return (await result).Bind(binder);
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="binder">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> BindAsync<TValue, TResult>(
        this OperationResult<TValue> result, 
        Func<TValue, Task<OperationResult<TResult>>> binder) 
        where TValue : notnull
        where TResult : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await binder(result.Value!),
            OperationResultType.Failure => OperationResult<TResult>.Failure(result.Error),
            OperationResultType.Cancelled => OperationResult<TResult>.Cancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Binds an operation result to an asynchronous function that returns a new operation result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The task containing the operation result to bind.</param>
    /// <param name="binder">The asynchronous function to apply to the value if the operation succeeded.</param>
    /// <returns>A task containing the operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TResult>> BindAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result, 
        Func<TValue, Task<OperationResult<TResult>>> binder) 
        where TValue : notnull
        where TResult : notnull
    {
        return await (await result).BindAsync(binder);
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The task containing the operation result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapErrorAsync<TValue>(
        this Task<OperationResult<TValue>> result, 
        Func<Error, Error> mapError) 
        where TValue : notnull
    {
        return (await result).MapError(mapError);
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="mapError">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapErrorAsync<TValue>(
        this OperationResult<TValue> result, 
        Func<Error, Task<Error>> mapError) 
        where TValue : notnull
    {
        return result.IsFailure ? OperationResult<TValue>.Failure(await mapError(result.Error)) : result;
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The task containing the operation result whose error to transform.</param>
    /// <param name="mapError">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapErrorAsync<TValue>(
        this Task<OperationResult<TValue>> result, 
        Func<Error, Task<Error>> mapError) 
        where TValue : notnull
    {
        return await (await result).MapErrorAsync(mapError);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapCancellationAsync<TValue>(
        this Task<OperationResult<TValue>> result, 
        Func<Cancelled, Cancelled> mapCancellation) 
        where TValue : notnull
    {
        return (await result).MapCancellation(mapCancellation);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapCancellationAsync<TValue>(
        this OperationResult<TValue> result, 
        Func<Cancelled, Task<Cancelled>> mapCancellation) 
        where TValue : notnull
    {
        return result.IsCancelled ? OperationResult<TValue>.Cancelled(await mapCancellation(result.Cancellation)) : result;
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult<TValue>> MapCancellationAsync<TValue>(
        this Task<OperationResult<TValue>> result, 
        Func<Cancelled, Task<Cancelled>> mapCancellation) 
        where TValue : notnull
    {
        return await (await result).MapCancellationAsync(mapCancellation);
    }

    #region MatchAsync
    
    /// <summary>
    /// Matches on the operation result, applying one of three functions depending on success, failure, or cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
        where TValue : notnull
    {
        return (await result).Match(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled) 
        where TValue : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.Value!),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
        where TValue : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.Value!),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.Value!),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }
    
    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
        where TValue : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
        where TValue : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result, 
        Func<TValue, Task<TResult>> onSuccess, 
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
        where TValue : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.Value!),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and cancellation or synchronous function on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result, 
        Func<TValue, Task<TResult>> onSuccess, 
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.Value!),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or asynchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result, 
        Func<TValue, TResult> onSuccess, 
        Func<Error, Task<TResult>> onFailure, 
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.Value!),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and failure or synchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
        where TValue : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and cancellation or synchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result, 
        Func<TValue, Task<TResult>> onSuccess, 
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or asynchronous functions on failure and cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result, 
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailure, 
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(result.Value!),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };  
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<OperationResult<TValue>> result,
        Func<TValue, Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
        where TValue : notnull
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    #endregion
}
