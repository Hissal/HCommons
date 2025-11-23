using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for Result{TValue} types.
/// </summary>
public static class Result1ExtensionsAsync {
    /// <summary>
    /// Maps a result value to a new value using the specified selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the result to transform.</param>
    /// <param name="selector">The function to apply to the value if the result is successful.</param>
    /// <returns>A task containing a result with the transformed value if successful, or the original error if failed.</returns>
    [Pure]
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, TResult> selector) where TValue : notnull where TResult : notnull {
        return (await resultTask).Select(selector);
    }
    
    /// <summary>
    /// Maps a result value to a new value using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the value if the result is successful.</param>
    /// <returns>A task containing a result with the transformed value if successful, or the original error if failed.</returns>
    [Pure]
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<TResult>> selectorAsync) where TValue : notnull where TResult : notnull {
        return result.IsSuccess ? Result<TResult>.Success(await selectorAsync(result.Value)) : Result<TResult>.Failure(result.Error);
    }
    
    /// <summary>
    /// Maps a result value to a new value using the specified asynchronous selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the value if the result is successful.</param>
    /// <returns>A task containing a result with the transformed value if successful, or the original error if failed.</returns>
    [Pure]
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Task<TResult>> selectorAsync) where TValue : notnull where TResult : notnull {
        return await (await resultTask).SelectAsync(selectorAsync);
    }
    
    /// <summary>
    /// Binds a result to a function that returns a new result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the result to bind.</param>
    /// <param name="binder">The function to apply to the value if the result is successful.</param>
    /// <returns>A task containing the result returned by the binder function if successful, or the original error if failed.</returns>
    [Pure]
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Result<TResult>> binder) where TValue : notnull where TResult : notnull {
        return (await resultTask).Bind(binder);
    }
    
    /// <summary>
    /// Binds a result to an asynchronous function that returns a new result.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the value if the result is successful.</param>
    /// <returns>A task containing the result returned by the binder function if successful, or the original error if failed.</returns>
    [Pure]
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<Result<TResult>>> binderAsync) where TValue : notnull where TResult : notnull {
        return result.IsSuccess ? await binderAsync(result.Value) : Result<TResult>.Failure(result.Error);
    }
    
    /// <summary>
    /// Binds a result to an asynchronous function that returns a new result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="resultTask">The task containing the result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the value if the result is successful.</param>
    /// <returns>A task containing the result returned by the binder function if successful, or the original error if failed.</returns>
    [Pure]
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Task<Result<TResult>>> binderAsync) where TValue : notnull where TResult : notnull {
        return await (await resultTask).BindAsync(binderAsync);
    }
    
    /// <summary>
    /// Transforms the error of a failed result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="resultTask">The task containing the result whose error to transform.</param>
    /// <param name="mapError">The function to apply to the error if the result is failed.</param>
    /// <returns>A task containing a result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Task<Result<TValue>> resultTask, Func<Error, Error> mapError) where TValue : notnull {
        return (await resultTask).MapError(mapError);
    }
    
    /// <summary>
    /// Transforms the error of a failed result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="result">The result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the result is failed.</param>
    /// <returns>A task containing a result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Result<TValue> result, Func<Error, Task<Error>> mapErrorAsync) where TValue : notnull {
        return result.IsFailure ? Result<TValue>.Failure(await mapErrorAsync(result.Error)) : result;
    }
    
    /// <summary>
    /// Transforms the error of a failed result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="resultTask">The task containing the result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to apply to the error if the result is failed.</param>
    /// <returns>A task containing a result with the transformed error if failed, or the original result if successful.</returns>
    [Pure]
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Task<Result<TValue>> resultTask, Func<Error, Task<Error>> mapErrorAsync) where TValue : notnull {
        return await (await resultTask).MapErrorAsync(mapErrorAsync);
    }
    
    /// <summary>
    /// Matches on the result, applying one of two functions depending on success or failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return (await resultTask).Match(onSuccess, onFailure);
    }
    
    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value) : onFailure(result.Error);
    }
    
    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value) : await onFailureAsync(result.Error);
    }
    
    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure);
    }
    
    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync);
    }
    
    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value) : await onFailureAsync(result.Error);
    }
    
    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync);
    }
}