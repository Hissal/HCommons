using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for OperationResult types.
/// </summary>
public static class OperationResultExtensions {
    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function.
    /// </summary>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the operation failed.</param>
    /// <returns>An operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static OperationResult MapError(this OperationResult result, Func<Error, Error> mapError) =>
        result.IsFailure ? OperationResult.Failure(mapError(result.Error)) : result;

    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="state">The state to pass to the mapError function.</param>
    /// <param name="mapError">The function to apply to the state and error if the operation failed.</param>
    /// <returns>An operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static OperationResult MapError<TState>(this OperationResult result, TState state, Func<TState, Error, Error> mapError) =>
        result.IsFailure ? OperationResult.Failure(mapError(state, result.Error)) : result;

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function.
    /// </summary>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>An operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static OperationResult MapCancellation(this OperationResult result, Func<Cancelled, Cancelled> mapCancellation) =>
        result.IsCancelled ? OperationResult.Cancelled(mapCancellation(result.Cancellation)) : result;

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="state">The state to pass to the mapCancellation function.</param>
    /// <param name="mapCancellation">The function to apply to the state and cancellation if the operation was cancelled.</param>
    /// <returns>An operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static OperationResult MapCancellation<TState>(this OperationResult result, TState state, Func<TState, Cancelled, Cancelled> mapCancellation) =>
        result.IsCancelled ? OperationResult.Cancelled(mapCancellation(state, result.Cancellation)) : result;

    /// <summary>
    /// Matches on the operation result and returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public static TResult Match<TResult>(
        this OperationResult result,
        Func<TResult> onSuccess, 
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled) 
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result with state and returns a value.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public static TResult Match<TState, TResult>(
        this OperationResult result,
        TState state,
        Func<TState, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure,
        Func<TState, Cancelled, TResult> onCancelled) 
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(state),
            OperationResultType.Failure => onFailure(state, result.Error),
            OperationResultType.Cancelled => onCancelled(state, result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Executes an action based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <param name="result">The operation result to switch on.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
    public static void Switch(
        this OperationResult result,
        Action onSuccess, 
        Action<Error> onFailure, 
        Action<Cancelled> onCancelled) 
    {
        if (result.IsSuccess) onSuccess();
        else if (result.IsFailure) onFailure(result.Error);
        else onCancelled(result.Cancellation);
    }

    /// <summary>
    /// Executes an action with state based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="result">The operation result to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
    public static void Switch<TState>(
        this OperationResult result,
        TState state, 
        Action<TState> onSuccess, 
        Action<TState, Error> onFailure,
        Action<TState, Cancelled> onCancelled) 
    {
        if (result.IsSuccess) onSuccess(state);
        else if (result.IsFailure) onFailure(state, result.Error);
        else onCancelled(state, result.Cancellation);
    }
}
