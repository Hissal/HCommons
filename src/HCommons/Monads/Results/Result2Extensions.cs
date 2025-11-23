using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for Result{TSuccess, TFailure} types.
/// </summary>
public static class Result2Extensions {
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
    [Pure]
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
    [Pure]
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
