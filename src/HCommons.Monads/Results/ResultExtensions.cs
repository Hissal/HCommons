namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for Result types.
/// </summary>
public static class ResultExtensions {
    /// <summary>
    /// Transforms the error of a failed result using the specified mapping function.
    /// </summary>
    /// <param name="result">The result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the result is failed.</param>
    /// <returns>A result with the transformed error if failed, or the original result if successful.</returns>
    public static Result MapError(this Result result, Func<Error, Error> mapError) =>
        result.IsFailure ? Result.Failure(mapError(result.Error)) : result;

    /// <summary>
    /// Transforms the error of a failed result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="result">The result whose error to transform.</param>
    /// <param name="state">The state to pass to the mapError function.</param>
    /// <param name="mapError">The function to apply to the state and error if the result is failed.</param>
    /// <returns>A result with the transformed error if failed, or the original result if successful.</returns>
    public static Result MapError<TState>(this Result result, TState state, Func<TState, Error, Error> mapError) =>
        result.IsFailure ? Result.Failure(mapError(state, result.Error)) : result;

    /// <summary>
    /// Matches on the result and returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <returns>The result of the executed function.</returns>
    public static TResult Match<TResult>(this Result result, Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        result.IsSuccess ? onSuccess() : onFailure(result.Error);

    /// <summary>
    /// Matches on the result with state and returns a value.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <returns>The result of the executed function.</returns>
    public static TResult Match<TState, TResult>(this Result result, TState state, Func<TState, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure) =>
        result.IsSuccess ? onSuccess(state) : onFailure(state, result.Error);

    /// <summary>
    /// Executes an action based on whether the operation succeeded or failed.
    /// </summary>
    /// <param name="result">The result to switch on.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public static void Switch(this Result result, Action onSuccess, Action<Error> onFailure) {
        if (result.IsSuccess) onSuccess();
        else onFailure(result.Error);
    }

    /// <summary>
    /// Executes an action with state based on whether the operation succeeded or failed.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="result">The result to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public static void Switch<TState>(this Result result, TState state, Action<TState> onSuccess, Action<TState, Error> onFailure) {
        if (result.IsSuccess) onSuccess(state);
        else onFailure(state, result.Error);
    }
}

