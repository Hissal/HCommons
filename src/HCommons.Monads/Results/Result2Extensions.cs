
namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for Result&lt;TSuccess, TFailure&gt; types.
/// </summary>
public static class Result2Extensions {
    /// <summary>
    /// Maps a result success value to a new value using the specified selector function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="selector">The function to apply to the success value if the result is successful.</param>
    /// <returns>A result containing the transformed success value if successful, or the original failure value if failed.</returns>
    public static Result<TResult, TFailure> Select<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result, 
        Func<TSuccess, TResult> selector) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull =>
        result.IsSuccess ? Result<TResult, TFailure>.Success(selector(result.Value!)) : Result<TResult, TFailure>.Failure(result.FailureValue!);

    /// <summary>
    /// Maps a result success value to a new value using the specified selector function with additional state.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="state">The state to pass to the selector function.</param>
    /// <param name="selector">The function to apply to the state and success value if the result is successful.</param>
    /// <returns>A result containing the transformed success value if successful, or the original failure value if failed.</returns>
    public static Result<TResult, TFailure> Select<TSuccess, TFailure, TState, TResult>(
        this Result<TSuccess, TFailure> result, 
        TState state, 
        Func<TState, TSuccess, TResult> selector) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull =>
        result.IsSuccess ? Result<TResult, TFailure>.Success(selector(state, result.Value!)) : Result<TResult, TFailure>.Failure(result.FailureValue!);

    /// <summary>
    /// Binds a result to a function that returns a new result, flattening nested results.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binder">The function to apply to the success value if the result is successful.</param>
    /// <returns>The result returned by the binder function if successful, or the original failure value if failed.</returns>
    public static Result<TResult, TFailure> Bind<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result, 
        Func<TSuccess, Result<TResult, TFailure>> binder) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull =>
        result.IsSuccess ? binder(result.Value!) : Result<TResult, TFailure>.Failure(result.FailureValue!);

    /// <summary>
    /// Binds a result to a function that returns a new result with additional state, flattening nested results.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="state">The state to pass to the binder function.</param>
    /// <param name="binder">The function to apply to the state and success value if the result is successful.</param>
    /// <returns>The result returned by the binder function if successful, or the original failure value if failed.</returns>
    public static Result<TResult, TFailure> Bind<TSuccess, TFailure, TState, TResult>(
        this Result<TSuccess, TFailure> result, 
        TState state, 
        Func<TState, TSuccess, Result<TResult, TFailure>> binder) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull =>
        result.IsSuccess ? binder(state, result.Value!) : Result<TResult, TFailure>.Failure(result.FailureValue!);

    /// <summary>
    /// Transforms the failure value of a failed result using the specified mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The result whose failure value to transform.</param>
    /// <param name="mapFailure">The function to apply to the failure value if the result is failed.</param>
    /// <returns>A result with the transformed failure value if failed, or the original result if successful.</returns>
    public static Result<TSuccess, TNewFailure> MapError<TSuccess, TFailure, TNewFailure>(
        this Result<TSuccess, TFailure> result, 
        Func<TFailure, TNewFailure> mapFailure) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TNewFailure : notnull =>
        result.IsFailure ? Result<TSuccess, TNewFailure>.Failure(mapFailure(result.FailureValue!)) : Result<TSuccess, TNewFailure>.Success(result.Value!);

    /// <summary>
    /// Transforms the failure value of a failed result using the specified mapping function with additional state.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The result whose failure value to transform.</param>
    /// <param name="state">The state to pass to the mapFailure function.</param>
    /// <param name="mapFailure">The function to apply to the state and failure value if the result is failed.</param>
    /// <returns>A result with the transformed failure value if failed, or the original result if successful.</returns>
    public static Result<TSuccess, TNewFailure> MapError<TSuccess, TFailure, TState, TNewFailure>(
        this Result<TSuccess, TFailure> result, 
        TState state, 
        Func<TState, TFailure, TNewFailure> mapFailure) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TNewFailure : notnull =>
        result.IsFailure ? Result<TSuccess, TNewFailure>.Failure(mapFailure(state, result.FailureValue!)) : Result<TSuccess, TNewFailure>.Success(result.Value!);

    /// <summary>
    /// Matches on the result and returns a value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <returns>The result of the executed function.</returns>
    public static TResult Match<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, TResult> onSuccess, 
        Func<TFailure, TResult> onFailure) 
        where TSuccess : notnull 
        where TFailure : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value!) : onFailure(result.FailureValue!);
    }
    
    /// <summary>
    /// Matches on the result with state and returns a value.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <returns>The result of the executed function.</returns>
    public static TResult Match<TSuccess, TFailure, TState, TResult>(
        this Result<TSuccess, TFailure> result,
        TState state,
        Func<TState, TSuccess, TResult> onSuccess, 
        Func<TState, TFailure, TResult> onFailure) 
        where TSuccess : notnull 
        where TFailure : notnull
    {
        return result.IsSuccess ? onSuccess(state, result.Value!) : onFailure(state, result.FailureValue!);
    }
    
    /// <summary>
    /// Executes an action based on whether the operation succeeded or failed.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <param name="result">The result to switch on.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public static void Switch<TSuccess, TFailure>(
        this Result<TSuccess, TFailure> result,
        Action<TSuccess> onSuccess, 
        Action<TFailure> onFailure) 
        where TSuccess : notnull 
        where TFailure : notnull
    {
        if (result.IsSuccess) onSuccess(result.Value!);
        else onFailure(result.FailureValue!);
    }
    
    /// <summary>
    /// Executes an action with state based on whether the operation succeeded or failed.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="result">The result to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public static void Switch<TSuccess, TFailure, TState>(
        this Result<TSuccess, TFailure> result,
        TState state,
        Action<TState, TSuccess> onSuccess, 
        Action<TState, TFailure> onFailure) 
        where TSuccess : notnull 
        where TFailure : notnull
    {
        if (result.IsSuccess) onSuccess(state, result.Value!);
        else onFailure(state, result.FailureValue!);
    }
}

