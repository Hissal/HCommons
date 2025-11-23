namespace HCommons.Monads;

public static class Result1ExtensionsAsync {
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, TResult> selector) where TValue : notnull where TResult : notnull {
        return (await resultTask).Select(selector);
    }
    
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<TResult>> selectorAsync) where TValue : notnull where TResult : notnull {
        return result.IsSuccess ? Result<TResult>.Success(await selectorAsync(result.Value)) : Result<TResult>.Failure(result.Error);
    }
    
    public static async Task<Result<TResult>> SelectAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Task<TResult>> selectorAsync) where TValue : notnull where TResult : notnull {
        return await (await resultTask).SelectAsync(selectorAsync);
    }
    
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Result<TResult>> binder) where TValue : notnull where TResult : notnull {
        return (await resultTask).Bind(binder);
    }
    
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Result<TValue> result, Func<TValue, Task<Result<TResult>>> binderAsync) where TValue : notnull where TResult : notnull {
        return result.IsSuccess ? await binderAsync(result.Value) : Result<TResult>.Failure(result.Error);
    }
    
    public static async Task<Result<TResult>> BindAsync<TValue, TResult>(this Task<Result<TValue>> resultTask, Func<TValue, Task<Result<TResult>>> binderAsync) where TValue : notnull where TResult : notnull {
        return await (await resultTask).BindAsync(binderAsync);
    }
    
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Task<Result<TValue>> resultTask, Func<Error, Error> mapError) where TValue : notnull {
        return (await resultTask).MapError(mapError);
    }
    
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Result<TValue> result, Func<Error, Task<Error>> mapErrorAsync) where TValue : notnull {
        return result.IsFailure ? Result<TValue>.Failure(await mapErrorAsync(result.Error)) : result;
    }
    
    public static async Task<Result<TValue>> MapErrorAsync<TValue>(this Task<Result<TValue>> resultTask, Func<Error, Task<Error>> mapErrorAsync) where TValue : notnull {
        return await (await resultTask).MapErrorAsync(mapErrorAsync);
    }
    
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return (await resultTask).Match(onSuccess, onFailure);
    }
    
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value) : onFailure(result.Error);
    }
    
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value) : await onFailureAsync(result.Error);
    }
    
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailure);
    }
    
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, TResult> onSuccess,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccess, onFailureAsync);
    }
    
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Result<TValue> result,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return result.IsSuccess ? await onSuccessAsync(result.Value) : await onFailureAsync(result.Error);
    }
    
    public static async Task<TResult> MatchAsync<TValue, TResult>(
        this Task<Result<TValue>> resultTask,
        Func<TValue, Task<TResult>> onSuccessAsync,
        Func<Error, Task<TResult>> onFailureAsync) where TValue : notnull
    {
        return await (await resultTask).MatchAsync(onSuccessAsync, onFailureAsync);
    }
}