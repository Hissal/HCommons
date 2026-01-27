
namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for OperationResult&lt;TValue&gt; types.
/// </summary>
public static class OperationResult1Extensions {
    /// <summary>
    /// Maps an operation result value to a new value using the specified selector function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="selector">The function to apply to the value if the operation succeeded.</param>
    /// <returns>An operation result containing the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    public static OperationResult<TResult> Select<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, TResult> selector)
        where TValue : notnull
        where TResult : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => OperationResult<TResult>.Success(selector(result.Value!)),
            OperationResultType.Failure => OperationResult<TResult>.Failure(result.Error),
            OperationResultType.Cancelled => OperationResult<TResult>.Cancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Maps an operation result value to a new value using the specified selector function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to transform.</param>
    /// <param name="state">The state to pass to the selector function.</param>
    /// <param name="selector">The function to apply to the state and value if the operation succeeded.</param>
    /// <returns>An operation result containing the transformed value if successful, or the original error or cancellation if failed or cancelled.</returns>
    public static OperationResult<TResult> Select<TState, TValue, TResult>(
        this OperationResult<TValue> result,
        TState state,
        Func<TState, TValue, TResult> selector)
        where TValue : notnull
        where TResult : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => OperationResult<TResult>.Success(selector(state, result.Value!)),
            OperationResultType.Failure => OperationResult<TResult>.Failure(result.Error),
            OperationResultType.Cancelled => OperationResult<TResult>.Cancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result, flattening nested results.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="binder">The function to apply to the value if the operation succeeded.</param>
    /// <returns>The operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    public static OperationResult<TResult> Bind<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, OperationResult<TResult>> binder)
        where TValue : notnull
        where TResult : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => binder(result.Value!),
            OperationResultType.Failure => OperationResult<TResult>.Failure(result.Error),
            OperationResultType.Cancelled => OperationResult<TResult>.Cancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Binds an operation result to a function that returns a new operation result with additional state, flattening nested results.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The operation result to bind.</param>
    /// <param name="state">The state to pass to the binder function.</param>
    /// <param name="binder">The function to apply to the state and value if the operation succeeded.</param>
    /// <returns>The operation result returned by the binder function if successful, or the original error or cancellation if failed or cancelled.</returns>
    public static OperationResult<TResult> Bind<TState, TValue, TResult>(
        this OperationResult<TValue> result,
        TState state,
        Func<TState, TValue, OperationResult<TResult>> binder)
        where TValue : notnull
        where TResult : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => binder(state, result.Value!),
            OperationResultType.Failure => OperationResult<TResult>.Failure(result.Error),
            OperationResultType.Cancelled => OperationResult<TResult>.Cancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the operation failed.</param>
    /// <returns>An operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    public static OperationResult<TValue> MapError<TValue>(
        this OperationResult<TValue> result,
        Func<Error, Error> mapError)
        where TValue : notnull 
    {
        return result.IsFailure ? OperationResult<TValue>.Failure(mapError(result.Error)) : result;
    }

    /// <summary>
    /// Transforms the error of a failed operation result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose error to transform.</param>
    /// <param name="state">The state to pass to the mapError function.</param>
    /// <param name="mapError">The function to apply to the state and error if the operation failed.</param>
    /// <returns>An operation result with the transformed error if failed, or the original result if successful or cancelled.</returns>
    public static OperationResult<TValue> MapError<TState, TValue>(
        this OperationResult<TValue> result,
        TState state,
        Func<TState, Error, Error> mapError)
        where TValue : notnull 
    {
        return result.IsFailure ? OperationResult<TValue>.Failure(mapError(state, result.Error)) : result;
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="mapCancellation">The function to apply to the cancellation if the operation was cancelled.</param>
    /// <returns>An operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    public static OperationResult<TValue> MapCancellation<TValue>(
        this OperationResult<TValue> result,
        Func<Cancelled, Cancelled> mapCancellation)
        where TValue : notnull 
    {
        return result.IsCancelled ? OperationResult<TValue>.Cancelled(mapCancellation(result.Cancellation)) : result;
    }

    /// <summary>
    /// Transforms the cancellation of a cancelled operation result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result whose cancellation to transform.</param>
    /// <param name="state">The state to pass to the mapCancellation function.</param>
    /// <param name="mapCancellation">The function to apply to the state and cancellation if the operation was cancelled.</param>
    /// <returns>An operation result with the transformed cancellation if cancelled, or the original result if successful or failed.</returns>
    public static OperationResult<TValue> MapCancellation<TState, TValue>(
        this OperationResult<TValue> result,
        TState state,
        Func<TState, Cancelled, Cancelled> mapCancellation)
        where TValue : notnull 
    {
        return result.IsCancelled
            ? OperationResult<TValue>.Cancelled(mapCancellation(state, result.Cancellation))
            : result;
    }

    /// <summary>
    /// Matches on the operation result and returns a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
    public static TResult Match<TValue, TResult>(
        this OperationResult<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled)
        where TValue : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(result.Value!),
            OperationResultType.Failure => onFailure(result.Error),
            OperationResultType.Cancelled => onCancelled(result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Matches on the operation result with state and returns a value.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The operation result to match on.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
    public static TResult Match<TState, TValue, TResult>(
        this OperationResult<TValue> result,
        TState state,
        Func<TState, TValue, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure,
        Func<TState, Cancelled, TResult> onCancelled)
        where TValue : notnull 
    {
        return result.Type switch {
            OperationResultType.Success => onSuccess(state, result.Value!),
            OperationResultType.Failure => onFailure(state, result.Error),
            OperationResultType.Cancelled => onCancelled(state, result.Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}")
        };
    }

    /// <summary>
    /// Executes an action based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result to switch on.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
    public static void Switch<TValue>(
        this OperationResult<TValue> result,
        Action<TValue> onSuccess,
        Action<Error> onFailure,
        Action<Cancelled> onCancelled)
        where TValue : notnull {
        switch (result.Type) {
            case OperationResultType.Success:
                onSuccess(result.Value!);
                break;
            case OperationResultType.Failure:
                onFailure(result.Error);
                break;
            case OperationResultType.Cancelled:
                onCancelled(result.Cancellation);
                break;
            default:
                throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}");
        }
    }

    /// <summary>
    /// Executes an action with state based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The operation result to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
    public static void Switch<TState, TValue>(
        this OperationResult<TValue> result,
        TState state,
        Action<TState, TValue> onSuccess,
        Action<TState, Error> onFailure,
        Action<TState, Cancelled> onCancelled)
        where TValue : notnull 
    {
        switch (result.Type) {
            case OperationResultType.Success:
                onSuccess(state, result.Value!);
                break;
            case OperationResultType.Failure:
                onFailure(state, result.Error);
                break;
            case OperationResultType.Cancelled:
                onCancelled(state, result.Cancellation);
                break;
            default:
                throw new InvalidOperationException($"Unknown OperationResultType: {result.Type}");
        }
    }
}
