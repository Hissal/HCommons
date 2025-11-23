using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for OperationResult types.
/// </summary>
public static class OperationResultExtensionsAsync {
    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <param name="result">The task containing the operation result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult> MapErrorAsync(
        this Task<OperationResult> result,
        Func<Error, Error> mapError)
    {
        return (await result).MapError(mapError);
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="mapError">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult> MapErrorAsync(
        this OperationResult result,
        Func<Error, Task<Error>> mapError) 
    {
        return result.IsFailure ? OperationResult.Failure(await mapError(result.Error)) : result;
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <param name="result">The task containing the operation result whose error to transform.</param>
    /// <param name="mapError">The asynchronous function to apply to the error if the operation failed.</param>
    /// <returns>A task containing an operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static async Task<OperationResult> MapErrorAsync(
        this Task<OperationResult> result,
        Func<Error, Task<Error>> mapError)
    {
        return await (await result).MapErrorAsync(mapError);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <param name="result">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult> MapCancellationAsync(
        this Task<OperationResult> result,
        Func<Cancelled, Cancelled> mapCancellation) 
    {
        return (await result).MapCancellation(mapCancellation);
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function.
    /// </summary>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult> MapCancellationAsync(
        this OperationResult result,
        Func<Cancelled, Task<Cancelled>> mapCancellation) 
    {
        return result.IsCancelled ? OperationResult.Cancelled(await mapCancellation(result.Cancellation)) : result;
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <param name="result">The task containing the operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The asynchronous function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>A task containing an operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static async Task<OperationResult> MapCancellationAsync(
        this Task<OperationResult> result,
        Func<Cancelled, Task<Cancelled>> mapCancellation)
    {
        return await (await result).MapCancellationAsync(mapCancellation);
    }

    #region MatchAsync
    
    /// <summary>
    /// Matches on the operation result, applying one of three functions depending on success, failure, or cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result,
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return (await result).Match(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<Task<TResult>> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled) 
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
                        _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<TResult> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }
    
    /// <summary>
    /// Matches on the operation result, applying an asynchronous function on success or synchronous functions on failure and cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result,
        Func<Task<TResult>> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or an asynchronous function on failure or synchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result,
        Func<TResult> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or synchronous function on failure or asynchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result,
        Func<TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and failure or synchronous function on cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result, 
        Func<Task<TResult>> onSuccess, 
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and cancellation or synchronous function on failure.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result, 
        Func<Task<TResult>> onSuccess, 
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or asynchronous functions on failure and cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result, 
        Func<TResult> onSuccess, 
        Func<Error, Task<TResult>> onFailure, 
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and failure or synchronous function on cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result,
        Func<Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, TResult> onCancelled)
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions on success and cancellation or synchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result, 
        Func<Task<TResult>> onSuccess, 
        Func<Error, TResult> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }

    /// <summary>
    /// Matches on the operation result, applying a synchronous function on success or asynchronous functions on failure and cancellation, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result, 
        Func<TResult> onSuccess,
        Func<Error, Task<TResult>> onFailure, 
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this OperationResult result,
        Func<Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return result.Type switch {
            OperationResultType.Success => await onSuccess(),
            OperationResultType.Failure => await onFailure(result.Error),
            OperationResultType.Cancelled => await onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };  
    }

    /// <summary>
    /// Matches on the operation result, applying asynchronous functions for all three cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The task containing the operation result to match on.</param>
    /// <param name="onSuccess">The asynchronous function to apply if the operation succeeded.</param>
    /// <param name="onFailure">The asynchronous function to apply if the operation failed.</param>
    /// <param name="onCancelled">The asynchronous function to apply if the operation was cancelled.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TResult>(
        this Task<OperationResult> result,
        Func<Task<TResult>> onSuccess,
        Func<Error, Task<TResult>> onFailure,
        Func<Cancelled, Task<TResult>> onCancelled)
    {
        return await (await result).MatchAsync(onSuccess, onFailure, onCancelled);
    }
    
    #endregion
    
}