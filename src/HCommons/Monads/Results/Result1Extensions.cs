namespace HCommons.Monads;

public static class Result1Extensions {
    public static Result<TResult> Select<TValue, TResult>(this Result<TValue> result, Func<TValue, TResult> selector) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? Result<TResult>.Success(selector(result.Value)) : Result<TResult>.Failure(result.Error);
    
    public static Result<TResult> Bind<TValue, TResult>(this Result<TValue> result, Func<TValue, Result<TResult>> binder) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? binder(result.Value) : Result<TResult>.Failure(result.Error);
    
    public static Result<TValue> MapError<TValue>(this Result<TValue> result, Func<Error, Error> mapError) where TValue : notnull =>
        result.IsFailure ? Result<TValue>.Failure(mapError(result.Error)) : result;
    
    public static TResult Match<TValue, TResult>(this Result<TValue> result,
        Func<TValue, TResult> onSuccess,
        Func<Error, TResult> onFailure) where TValue : notnull
    {
        return result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);
    }
    
    public static void Switch<TValue>(this Result<TValue> result,
        Action<TValue> onSuccess,
        Action<Error> onFailure) where TValue : notnull
    {
        if (result.IsSuccess) onSuccess(result.Value);
        else onFailure(result.Error);
    }
    
    // ===== Stateful Overloads ===== //
    
    public static Result<TResult> Select<TState, TValue, TResult>(this Result<TValue> result, TState state, Func<TState, TValue, TResult> selector) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? Result<TResult>.Success(selector(state, result.Value)) : Result<TResult>.Failure(result.Error);
    
    public static Result<TResult> Bind<TState, TValue, TResult>(this Result<TValue> result, TState state, Func<TState, TValue, Result<TResult>> binder) where TValue : notnull where TResult : notnull =>
        result.IsSuccess ? binder(state, result.Value) : Result<TResult>.Failure(result.Error);
    
    public static Result<TValue> MapError<TState, TValue>(this Result<TValue> result, TState state, Func<TState, Error, Error> mapError) where TValue : notnull =>
        result.IsFailure ? Result<TValue>.Failure(mapError(state, result.Error)) : result;
    
    public static TResult Match<TState, TValue, TResult>(this Result<TValue> result,
        TState state,
        Func<TState, TValue, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure) where TValue : notnull
    {
        return result.IsSuccess ? onSuccess(state, result.Value) : onFailure(state, result.Error);
    }
    
    public static void Switch<TState, TValue>(this Result<TValue> result,
        TState state,
        Action<TState, TValue> onSuccess,
        Action<TState, Error> onFailure) where TValue : notnull
    {
        if (result.IsSuccess) onSuccess(state, result.Value);
        else onFailure(state, result.Error);
    }
}