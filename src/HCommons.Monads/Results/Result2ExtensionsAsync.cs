
namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for Result&lt;TSuccess, TFailure&gt; types.
/// </summary>
public static class Result2ExtensionsAsync {
    /// <summary>
    /// Maps a result success value to a new value using the specified selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the result to transform.</param>
    /// <param name="selector">The function to apply to the success value if the result is successful.</param>
    /// <returns>A task containing a result with the transformed success value if successful, or the original failure value if failed.</returns>
    public static async Task<Result<TResult, TFailure>> SelectAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask, 
        Func<TSuccess, TResult> selector) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull {
        return (await resultTask).Select(selector);
    }

    /// <summary>
    /// Maps a result success value to a new value using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the success value if the result is successful.</param>
    /// <returns>A task containing a result with the transformed success value if successful, or the original failure value if failed.</returns>
    public static async Task<Result<TResult, TFailure>> SelectAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result, 
        Func<TSuccess, Task<TResult>> selectorAsync) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull {
        return result.IsSuccess ? Result<TResult, TFailure>.Success(await selectorAsync(result.Value!)) : Result<TResult, TFailure>.Failure(result.FailureValue!);
    }

    /// <summary>
    /// Maps a result success value to a new value using the specified asynchronous selector function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the result to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the success value if the result is successful.</param>
    /// <returns>A task containing a result with the transformed success value if successful, or the original failure value if failed.</returns>
    public static async Task<Result<TResult, TFailure>> SelectAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask, 
        Func<TSuccess, Task<TResult>> selectorAsync) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull {
        return await (await resultTask).SelectAsync(selectorAsync);
    }

    /// <summary>
    /// Binds a result to a function that returns a new result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the result to bind.</param>
    /// <param name="binder">The function to apply to the success value if the result is successful.</param>
    /// <returns>A task containing the result returned by the binder function if successful, or the original failure value if failed.</returns>
    public static async Task<Result<TResult, TFailure>> BindAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask, 
        Func<TSuccess, Result<TResult, TFailure>> binder) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull {
        return (await resultTask).Bind(binder);
    }

    /// <summary>
    /// Binds a result to an asynchronous function that returns a new result.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="result">The result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the success value if the result is successful.</param>
    /// <returns>A task containing the result returned by the binder function if successful, or the original failure value if failed.</returns>
    public static async Task<Result<TResult, TFailure>> BindAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result, 
        Func<TSuccess, Task<Result<TResult, TFailure>>> binderAsync) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull {
        return result.IsSuccess ? await binderAsync(result.Value!) : Result<TResult, TFailure>.Failure(result.FailureValue!);
    }

    /// <summary>
    /// Binds a result to an asynchronous function that returns a new result, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the source success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the result success value.</typeparam>
    /// <param name="resultTask">The task containing the result to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the success value if the result is successful.</param>
    /// <returns>A task containing the result returned by the binder function if successful, or the original failure value if failed.</returns>
    public static async Task<Result<TResult, TFailure>> BindAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask, 
        Func<TSuccess, Task<Result<TResult, TFailure>>> binderAsync) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TResult : notnull {
        return await (await resultTask).BindAsync(binderAsync);
    }

    /// <summary>
    /// Transforms the failure value of a failed result using the specified mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="resultTask">The task containing the result whose failure value to transform.</param>
    /// <param name="mapFailure">The function to apply to the failure value if the result is failed.</param>
    /// <returns>A task containing a result with the transformed failure value if failed, or the original result if successful.</returns>
    public static async Task<Result<TSuccess, TNewFailure>> MapErrorAsync<TSuccess, TFailure, TNewFailure>(
        this Task<Result<TSuccess, TFailure>> resultTask, 
        Func<TFailure, TNewFailure> mapFailure) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TNewFailure : notnull {
        return (await resultTask).MapError(mapFailure);
    }

    /// <summary>
    /// Transforms the failure value of a failed result using the specified asynchronous mapping function.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="result">The result whose failure value to transform.</param>
    /// <param name="mapFailureAsync">The asynchronous function to apply to the failure value if the result is failed.</param>
    /// <returns>A task containing a result with the transformed failure value if failed, or the original result if successful.</returns>
    public static async Task<Result<TSuccess, TNewFailure>> MapErrorAsync<TSuccess, TFailure, TNewFailure>(
        this Result<TSuccess, TFailure> result, 
        Func<TFailure, Task<TNewFailure>> mapFailureAsync) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TNewFailure : notnull {
        return result.IsFailure ? Result<TSuccess, TNewFailure>.Failure(await mapFailureAsync(result.FailureValue!)) : Result<TSuccess, TNewFailure>.Success(result.Value!);
    }

    /// <summary>
    /// Transforms the failure value of a failed result using the specified asynchronous mapping function, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the source failure value.</typeparam>
    /// <typeparam name="TNewFailure">The type of the new failure value.</typeparam>
    /// <param name="resultTask">The task containing the result whose failure value to transform.</param>
    /// <param name="mapFailureAsync">The asynchronous function to apply to the failure value if the result is failed.</param>
    /// <returns>A task containing a result with the transformed failure value if failed, or the original result if successful.</returns>
    public static async Task<Result<TSuccess, TNewFailure>> MapErrorAsync<TSuccess, TFailure, TNewFailure>(
        this Task<Result<TSuccess, TFailure>> resultTask, 
        Func<TFailure, Task<TNewFailure>> mapFailureAsync) 
        where TSuccess : notnull 
        where TFailure : notnull
        where TNewFailure : notnull {
        return await (await resultTask).MapErrorAsync(mapFailureAsync);
    }

    /// <summary>
    /// Matches on the result, applying one of two functions depending on success or failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, TResult> onFailure)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return (await resultTask).Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, TResult> onFailure)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value!) : onFailure(result.FailureValue!);
    }

    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value!) : await onFailureAsync(result.FailureValue!);
    }

    /// <summary>
    /// Matches on the result, applying an asynchronous function on success or a synchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailure">The function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, TResult> onFailure)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure);
    }

    /// <summary>
    /// Matches on the result, applying a synchronous function on success or an asynchronous function on failure, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccess">The function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, TResult> onSuccess,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync);
    }

    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="result">The result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Result<TSuccess, TFailure> result,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value!) : await onFailureAsync(result.FailureValue!);
    }

    /// <summary>
    /// Matches on the result, applying asynchronous functions for both success and failure cases, awaiting the result task.
    /// </summary>
    /// <typeparam name="TSuccess">The type of the success value.</typeparam>
    /// <typeparam name="TFailure">The type of the failure value.</typeparam>
    /// <typeparam name="TResult">The type of the match result.</typeparam>
    /// <param name="resultTask">The task containing the result to match on.</param>
    /// <param name="onSuccessAsync">The asynchronous function to apply if the result is successful.</param>
    /// <param name="onFailureAsync">The asynchronous function to apply if the result is failed.</param>
    /// <returns>A task containing the result of applying the appropriate function.</returns>
    public static async Task<TResult> MatchAsync<TSuccess, TFailure, TResult>(
        this Task<Result<TSuccess, TFailure>> resultTask,
        Func<TSuccess, Task<TResult>> onSuccessAsync,
        Func<TFailure, Task<TResult>> onFailureAsync)
        where TSuccess : notnull
        where TFailure : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync);
    }
}

