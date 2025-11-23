using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for Result{TValue} types.
/// </summary>
public static class Result1Extensions {
    /// <summary>
    /// Maps a result value to a new value using the specified selector function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="selector">The function to apply to the value if the result is successful.</param>
    /// <returns>A result containing the transformed value if successful, or the original error if failed.</returns>
    [Pure]
    public static Result<TResult> Select<TValue, TResult>(this Result<TValue> result, Func<TValue, TResult> selector) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? Result<TResult>.Success(selector(result.Value)) : Result<TResult>.Failure(result.Error);
    
    /// <summary>
    /// Binds a result to a function that returns a new result, flattening nested results.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binder">The function to apply to the value if the result is successful.</param>
    /// <returns>The result returned by the binder function if successful, or the original error if failed.</returns>
    [Pure]
    public static Result<TResult> Bind<TValue, TResult>(this Result<TValue> result, Func<TValue, Result<TResult>> binder) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? binder(result.Value) : Result<TResult>.Failure(result.Error);
    
    /// <summary>
    /// Transforms the error of a failed result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the result is failed.</param>
    /// <returns>A result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static Result<TValue> MapError<TValue>(this Result<TValue> result, Func<Error, Error> mapError) where TValue : notnull =>
        result.IsFailure ? Result<TValue>.Failure(mapError(result.Error)) : result;
    
    /// <summary>
    /// Matches on the result, applying one of two functions depending on success or failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>The result of applying the appropriate function.</returns>
    [Pure]
    public static TResult Match<TValue, TResult>(this Result<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }
    
    /// <summary>
    /// Executes one of two actions depending on whether the result is successful or failed.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The result to switch on.</param>
    /// <param name="onSuccess">The action to execute if the result is successful.</param>
    /// <param name="onFailure">The action to execute if the result is failed.</param>
    public static void Switch<TValue>(this Result<TValue> result,
        Action<TValue> onSuccess,
        Action<Error> onFailure) where TValue : notnull
    {
        if (result.IsSuccess) onSuccess(result.Value);
        else onFailure(result.Error);
    }
    
    // ===== Stateful Overloads ===== //
    
    /// <summary>
    /// Maps a result value to a new value using the specified selector function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="state">The state to pass to the selector function.</param>
    /// <param name="selector">The function to apply to the state and value if the result is successful.</param>
    /// <returns>A result containing the transformed value if successful, or the original error if failed.</returns>
    [Pure]
    public static Result<TResult> Select<TState, TValue, TResult>(this Result<TValue> result, TState state, Func<TState, TValue, TResult> selector) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? Result<TResult>.Success(selector(state, result.Value)) : Result<TResult>.Failure(result.Error);
    
    /// <summary>
    /// Binds a result to a function that returns a new result with additional state, flattening nested results.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="state">The state to pass to the binder function.</param>
    /// <param name="binder">The function to apply to the state and value if the result is successful.</param>
    /// <returns>The result returned by the binder function if successful, or the original error if failed.</returns>
    [Pure]
    public static Result<TResult> Bind<TState, TValue, TResult>(this Result<TValue> result, TState state, Func<TState, TValue, Result<TResult>> binder) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? binder(state, result.Value) : Result<TResult>.Failure(result.Error);
    
    /// <summary>
    /// Transforms the error of a failed result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The result whose error to transform.</param>
    /// <param name="state">The state to pass to the mapError function.</param>
    /// <param name="mapError">The function to apply to the state and error if the result is failed.</param>
    /// <returns>A result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static Result<TValue> MapError<TState, TValue>(this Result<TValue> result, TState state, Func<TState, Error, Error> mapError) where TValue : notnull =>
        result.IsFailure ? Result<TValue>.Failure(mapError(state, result.Error)) : result;
    
    /// <summary>
    /// Matches on the result with additional state, applying one of two functions depending on success or failure.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to apply to the state and value if the result is successful.</param>
    /// <param name="onFailure">The function to apply to the state and error if the result is failed.</param>
    /// <returns>The result of applying the appropriate function.</returns>
    [Pure]
    public static TResult Match<TState, TValue, TResult>(this Result<TValue> result,
        TState state,
        Func<TState, TValue, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure) where TValue : notnull
    {
        return result.IsSuccess ? onSuccess(state, result.Value) : onFailure(state, result.Error);
    }
    
    /// <summary>
    /// Executes one of two actions with additional state depending on whether the result is successful or failed.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The result to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute with the state and value if the result is successful.</param>
    /// <param name="onFailure">The action to execute with the state and error if the result is failed.</param>
    public static void Switch<TState, TValue>(this Result<TValue> result,
        TState state,
        Action<TState, TValue> onSuccess,
        Action<TState, Error> onFailure) where TValue : notnull
    {
        if (result.IsSuccess) onSuccess(state, result.Value);
        else onFailure(state, result.Error);
    }
}