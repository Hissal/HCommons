namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for Result{TValue} types.
/// </summary>
public static class Result1ExtensionsAsync {
    /// <summary>
    /// Awaits a Result task and transforms its success value using the specified selector.
    /// </summary>
    /// <typeparam name="TValue">The type of the source result value.</typeparam>
    /// <typeparam name="TResult">The type of the transformed result value.</typeparam>
    /// <param name="resultTask">The task returning the result to transform.</param>
    /// <param name="selector">The function to transform the success value.</param>
    /// <returns>A task representing the transformed result.</returns>
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, TResult> selector) where TValue : notnull where TResult : notnull {
        return (await resultTask).Select(selector);
    }
    
    /// <summary>
    /// Transforms a result's success value using an asynchronous selector function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source result value.</typeparam>
    /// <typeparam name="TResult">The type of the transformed result value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to transform the success value.</param>
    /// <returns>A task representing the transformed result.</returns>
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<TResult>> selectorAsync) where TValue : notnull where TResult : notnull {
        return result.IsSuccess ? Result<TResult>.Success(await selectorAsync(result.Value)) : Result<TResult>.Failure(result.Error);
    }
    
    /// <summary>
    /// Awaits a Result task and transforms its success value using an asynchronous selector function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source result value.</typeparam>
    /// <typeparam name="TResult">The type of the transformed result value.</typeparam>
    /// <param name="resultTask">The task returning the result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to transform the success value.</param>
    /// <returns>A task representing the transformed result.</returns>
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Task<TResult>> selectorAsync) where TValue : notnull where TResult : notnull {
        return await (await resultTask).SelectAsync(selectorAsync);
    }
    
    /// <summary>
    /// Awaits a Result task and binds its success value to another result using the specified binder.
    /// </summary>
    /// <typeparam name="TValue">The type of the source result value.</typeparam>
    /// <typeparam name="TResult">The type of the bound result value.</typeparam>
    /// <param name="resultTask">The task returning the result to bind.</param>
    /// <param name="binder">The function to bind the success value to another result.</param>
    /// <returns>A task representing the bound result.</returns>
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Result<TResult>> binder) where TValue : notnull where TResult : notnull {
        return (await resultTask).Bind(binder);
    }
    
    /// <summary>
    /// Binds a result's success value to another result using an asynchronous binder function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source result value.</typeparam>
    /// <typeparam name="TResult">The type of the bound result value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to bind the success value to another result.</param>
    /// <returns>A task representing the bound result.</returns>
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<Result<TResult>>> binderAsync) where TValue : notnull where TResult : notnull {
        return result.IsSuccess ? await binderAsync(result.Value) : Result<TResult>.Failure(result.Error);
    }
    
    /// <summary>
    /// Awaits a Result task and binds its success value to another result using an asynchronous binder function.
    /// </summary>
    /// <typeparam name="TValue">The type of the source result value.</typeparam>
    /// <typeparam name="TResult">The type of the bound result value.</typeparam>
    /// <param name="resultTask">The task returning the result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to bind the success value to another result.</param>
    /// <returns>A task representing the bound result.</returns>
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Task<Result<TResult>>> binderAsync) where TValue : notnull where TResult : notnull {
        return await (await resultTask).BindAsync(binderAsync);
    }
    
    /// <summary>
    /// Awaits a Result task and transforms its error using the specified mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="resultTask">The task returning the result whose error to transform.</param>
    /// <param name="mapError">The function to transform the error.</param>
    /// <returns>A task representing the result with the transformed error.</returns>
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Task<Result<TValue>> resultTask, Func<Error, Error> mapError) where TValue : notnull {
        return (await resultTask).MapError(mapError);
    }
    
    /// <summary>
    /// Transforms a result's error using an asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="result">The result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to transform the error.</param>
    /// <returns>A task representing the result with the transformed error.</returns>
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Result<TValue> result, Func<Error, Task<Error>> mapErrorAsync) where TValue : notnull {
        return result.IsFailure ? Result<TValue>.Failure(await mapErrorAsync(result.Error)) : result;
    }
    
    /// <summary>
    /// Awaits a Result task and transforms its error using an asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <param name="resultTask">The task returning the result whose error to transform.</param>
    /// <param name="mapErrorAsync">The asynchronous function to transform the error.</param>
    /// <returns>A task representing the result with the transformed error.</returns>
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Task<Result<TValue>> resultTask, Func<Error, Task<Error>> mapErrorAsync) where TValue : notnull {
        return await (await resultTask).MapErrorAsync(mapErrorAsync);
    }
    
    /// <summary>
    /// Awaits a Result task and matches its state, applying one of two functions based on whether it's a success or failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="resultTask">The task returning the result to match.</param>
    /// <param name="onSuccess">The function to apply if the result is a success.</param>
    /// <param name="onFailure">The function to apply if the result is a failure.</param>
    /// <returns>A task representing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return (await resultTask).Match(onSuccess, onFailure);
    }
    
    /// <summary>
    /// Matches a result's state, applying an asynchronous function on success or a synchronous function on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is a success.</param>
    /// <param name="onFailure">The function to apply if the result is a failure.</param>
    /// <returns>A task representing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value) : onFailure(result.Error);
    }
    
    /// <summary>
    /// Matches a result's state, applying a synchronous function on success or an asynchronous function on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccess">The function to apply if the result is a success.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is a failure.</param>
    /// <returns>A task representing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value) : await onFailureAsync(result.Error);
    }
    
    /// <summary>
    /// Awaits a Result task and matches its state, applying an asynchronous function on success or a synchronous function on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="resultTask">The task returning the result to match.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is a success.</param>
    /// <param name="onFailure">The function to apply if the result is a failure.</param>
    /// <returns>A task representing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure);
    }
    
    /// <summary>
    /// Awaits a Result task and matches its state, applying a synchronous function on success or an asynchronous function on failure.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="resultTask">The task returning the result to match.</param>
    /// <param name="onSuccess">The function to apply if the result is a success.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is a failure.</param>
    /// <returns>A task representing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync);
    }
    
    /// <summary>
    /// Matches a result's state, applying asynchronous functions for both success and failure cases.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="result">The result to match.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is a success.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is a failure.</param>
    /// <returns>A task representing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value) : await onFailureAsync(result.Error);
    }
    
    /// <summary>
    /// Awaits a Result task and matches its state, applying asynchronous functions for both success and failure cases.
    /// </summary>
    /// <typeparam name="TValue">The type of the result value.</typeparam>
    /// <typeparam name="TResult">The type of the return value.</typeparam>
    /// <param name="resultTask">The task returning the result to match.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is a success.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is a failure.</param>
    /// <returns>A task representing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync);
    }
}