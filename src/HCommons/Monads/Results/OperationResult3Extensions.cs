using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for OperationResult&lt;TSuccess, TFailure, TCancelled&gt; types.
/// </summary>
public static class OperationResult3Extensions {
    /// <summary>
    /// Maps an operation result success value to a new value using the specified selector function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="selector">The function to apply to the success value if the operation succeeded.</param>
    /// <returns>An operation result containing the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static OperationResult<TResult, TFailure, TCancelled> Select<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, TResult> selector)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull =>
        result.Type switch {
            OperationResultType.Success => OperationResult<TResult, TFailure, TCancelled>.Success(selector(result.SuccessValue!)),
            OperationResultType.Failure => OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Maps an operation result success value to a new value using the specified selector function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="state">The state to pass to the selector function.</param>
    /// <param name="selector">The function to apply to the state and success value if the operation succeeded.</param>
    /// <returns>An operation result containing the transformed success value if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static OperationResult<TResult, TFailure, TCancelled> Select<TState, TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        TState state,
        Func<TState, TSuccess, TResult> selector)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull =>
        result.Type switch {
            OperationResultType.Success => OperationResult<TResult, TFailure, TCancelled>.Success(selector(state, result.SuccessValue!)),
            OperationResultType.Failure => OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result, flattening nested results.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="binder">The function to apply to the success value if the operation succeeded.</param>
    /// <returns>The operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static OperationResult<TResult, TFailure, TCancelled> Bind<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TSuccess, OperationResult<TResult, TFailure, TCancelled>> binder)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull =>
        result.Type switch {
            OperationResultType.Success => binder(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result with additional state, flattening nested results.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="state">The state to pass to the binder function.</param>
    /// <param name="binder">The function to apply to the state and success value if the operation succeeded.</param>
    /// <returns>The operation result returned by the binder function if successful, or the original failure or cancellation value if failed or cancelled.</returns>
    [Pure]
    public static OperationResult<TResult, TFailure, TCancelled> Bind<TState, TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        TState state,
        Func<TState, TSuccess, OperationResult<TResult, TFailure, TCancelled>> binder)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TResult : notnull =>
        result.Type switch {
            OperationResultType.Success => binder(state, result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TResult, TFailure, TCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TResult, TFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Transforms the failure value of a failed operation result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The operation result whose failure value to transform.</param>
    /// <param name="mapFailure">The function to apply to the failure value if the operation failed.</param>
    /// <returns>An operation result with the transformed failure value if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static OperationResult<TSuccess, TNewFailure, TCancelled> MapError<TSuccess, TFailure, TCancelled, TNewFailure>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TFailure, TNewFailure> mapFailure)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull =>
        result.Type switch {
            OperationResultType.Success => OperationResult<TSuccess, TNewFailure, TCancelled>.Success(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TSuccess, TNewFailure, TCancelled>.Failure(mapFailure(result.FailureValue!)),
            OperationResultType.Cancelled => OperationResult<TSuccess, TNewFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Transforms the failure value of a failed operation result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The operation result whose failure value to transform.</param>
    /// <param name="state">The state to pass to the mapFailure function.</param>
    /// <param name="mapFailure">The function to apply to the state and failure value if the operation failed.</param>
    /// <returns>An operation result with the transformed failure value if failed, or the original result if successful or cancelled.</returns>
    [Pure]
    public static OperationResult<TSuccess, TNewFailure, TCancelled> MapError<TState, TSuccess, TFailure, TCancelled, TNewFailure>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        TState state,
        Func<TState, TFailure, TNewFailure> mapFailure)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewFailure : notnull =>
        result.Type switch {
            OperationResultType.Success => OperationResult<TSuccess, TNewFailure, TCancelled>.Success(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TSuccess, TNewFailure, TCancelled>.Failure(mapFailure(state, result.FailureValue!)),
            OperationResultType.Cancelled => OperationResult<TSuccess, TNewFailure, TCancelled>.Cancelled(result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="result">The operation result whose cancellation value to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation value if the operation was cancelled.</param>
    /// <returns>An operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static OperationResult<TSuccess, TFailure, TNewCancelled> MapCancellation<TSuccess, TFailure, TCancelled, TNewCancelled>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        Func<TCancelled, TNewCancelled> mapCancellation)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull =>
        result.Type switch {
            OperationResultType.Success => OperationResult<TSuccess, TFailure, TNewCancelled>.Success(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TSuccess, TFailure, TNewCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TSuccess, TFailure, TNewCancelled>.Cancelled(mapCancellation(result.CancelledValue!)),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Transforms the cancellation value of a cancelled operation result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the source cancellation value.</typeparam>
    /// <typeparam name="TNewCancelled">The type of the new cancellation value.</typeparam>
    /// <param name="result">The operation result whose cancellation value to transform.</param>
    /// <param name="state">The state to pass to the mapCancellation function.</param>
    /// <param name="mapCancellation">The function to apply to the state and cancellation value if the operation was cancelled.</param>
    /// <returns>An operation result with the transformed cancellation value if cancelled, or the original result if successful or failed.</returns>
    [Pure]
    public static OperationResult<TSuccess, TFailure, TNewCancelled> MapCancellation<TState, TSuccess, TFailure, TCancelled, TNewCancelled>(
        this OperationResult<TSuccess, TFailure, TCancelled> result, 
        TState state,
        Func<TState, TCancelled, TNewCancelled> mapCancellation)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
        where TNewCancelled : notnull =>
        result.Type switch {
            OperationResultType.Success => OperationResult<TSuccess, TFailure, TNewCancelled>.Success(result.SuccessValue!),
            OperationResultType.Failure => OperationResult<TSuccess, TFailure, TNewCancelled>.Failure(result.FailureValue!),
            OperationResultType.Cancelled => OperationResult<TSuccess, TFailure, TNewCancelled>.Cancelled(mapCancellation(state, result.CancelledValue!)),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };

    /// <summary>
    /// Matches on the operation result and returns a value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public static TResult Match<TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Func<TSuccess, TResult> onSuccess, 
        Func<TFailure, TResult> onFailure, 
        Func<TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.SuccessValue!),
            OperationResultType.Failure => onFailure(result.FailureValue!),
            OperationResultType.Cancelled => onCancelled(result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };
    }

    /// <summary>
    /// Matches on the operation result with state and returns a value.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public static TResult Match<TState, TSuccess, TFailure, TCancelled, TResult>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        TState state,
        Func<TState, TSuccess, TResult> onSuccess, 
        Func<TState, TFailure, TResult> onFailure, 
        Func<TState, TCancelled, TResult> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(state, result.SuccessValue!),
            OperationResultType.Failure => onFailure(state, result.FailureValue!),
            OperationResultType.Cancelled => onCancelled(state, result.CancelledValue!),
            _ => throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType")
        };
    }

    /// <summary>
    /// Executes an action based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <param name="result">The operation result to switch on.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
    public static void Switch<TSuccess, TFailure, TCancelled>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        Action<TSuccess> onSuccess, 
        Action<TFailure> onFailure, 
        Action<TCancelled> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        switch (result.Type) {
            case OperationResultType.Success:
                onSuccess(result.SuccessValue!);
                break;
            case OperationResultType.Failure:
                onFailure(result.FailureValue!);
                break;
            case OperationResultType.Cancelled:
                onCancelled(result.CancelledValue!);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType");
        }
    }

    /// <summary>
    /// Executes an action with state based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
    /// <param name="result">The operation result to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
    public static void Switch<TState, TSuccess, TFailure, TCancelled>(
        this OperationResult<TSuccess, TFailure, TCancelled> result,
        TState state,
        Action<TState, TSuccess> onSuccess, 
        Action<TState, TFailure> onFailure, 
        Action<TState, TCancelled> onCancelled)
        where TSuccess : notnull
        where TFailure : notnull
        where TCancelled : notnull
    {
        switch (result.Type) {
            case OperationResultType.Success:
                onSuccess(state, result.SuccessValue!);
                break;
            case OperationResultType.Failure:
                onFailure(state, result.FailureValue!);
                break;
            case OperationResultType.Cancelled:
                onCancelled(state, result.CancelledValue!);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "Unknown OperationResultType");
        }
    }
}
