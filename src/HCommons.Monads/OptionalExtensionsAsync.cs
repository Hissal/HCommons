namespace HCommons.Monads;

/// <summary>
/// Provides asynchronous extension methods for Optional{T} types.
/// </summary>
public static class OptionalExtensionsAsync {
    /// <summary>
    /// Projects the value into a new form using the specified selector function, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The task containing the optional to transform.</param>
    /// <param name="selector">The function to apply to the value if present.</param>
    /// <returns>A task containing an optional with the transformed value if present, or an empty optional if no value is present.</returns>
    public static async Task<Optional<TResult>> SelectAsync<T, TResult>(this Task<Optional<T>> optionalTask, Func<T, TResult> selector) where T : notnull where TResult : notnull {
        return (await optionalTask).Select(selector);
    }
    
    /// <summary>
    /// Projects the value into a new form using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the value if present.</param>
    /// <returns>A task containing an optional with the transformed value if present, or an empty optional if no value is present.</returns>
    public static async Task<Optional<TResult>> SelectAsync<T, TResult>(this Optional<T> optional, Func<T, Task<TResult>> selectorAsync) where T : notnull where TResult : notnull {
        return optional.HasValue ? Optional<TResult>.Some(await selectorAsync(optional.Value)) : Optional<TResult>.None();
    }
    
    /// <summary>
    /// Projects the value into a new form using the specified asynchronous selector function, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The task containing the optional to transform.</param>
    /// <param name="selectorAsync">The asynchronous function to apply to the value if present.</param>
    /// <returns>A task containing an optional with the transformed value if present, or an empty optional if no value is present.</returns>
    public static async Task<Optional<TResult>> SelectAsync<T, TResult>(this Task<Optional<T>> optionalTask, Func<T, Task<TResult>> selectorAsync) where T : notnull where TResult : notnull {
        return await (await optionalTask).SelectAsync(selectorAsync);
    }
    
    /// <summary>
    /// Projects the value into a new optional using the specified binder function, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The task containing the optional to bind.</param>
    /// <param name="binder">The function to apply to the value if present.</param>
    /// <returns>A task containing the result of applying the binder, or an empty optional if no value is present.</returns>
    public static async Task<Optional<TResult>> BindAsync<T, TResult>(this Task<Optional<T>> optionalTask, Func<T, Optional<TResult>> binder) where T : notnull where TResult : notnull {
        return (await optionalTask).Bind(binder);
    }
    
    /// <summary>
    /// Projects the value into a new optional using the specified asynchronous binder function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the value if present.</param>
    /// <returns>A task containing the result of applying the binder, or an empty optional if no value is present.</returns>
    public static async Task<Optional<TResult>> BindAsync<T, TResult>(this Optional<T> optional, Func<T, Task<Optional<TResult>>> binderAsync) where T : notnull where TResult : notnull {
        return optional.HasValue ? await binderAsync(optional.Value) : Optional<TResult>.None();
    }
    
    /// <summary>
    /// Projects the value into a new optional using the specified asynchronous binder function, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The task containing the optional to bind.</param>
    /// <param name="binderAsync">The asynchronous function to apply to the value if present.</param>
    /// <returns>A task containing the result of applying the binder, or an empty optional if no value is present.</returns>
    public static async Task<Optional<TResult>> BindAsync<T, TResult>(this Task<Optional<T>> optionalTask, Func<T, Task<Optional<TResult>>> binderAsync) where T : notnull where TResult : notnull {
        return await (await optionalTask).BindAsync(binderAsync);
    }
    
    /// <summary>
    /// Filters the value based on a predicate, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optionalTask">The task containing the optional to filter.</param>
    /// <param name="predicate">The predicate to test the value.</param>
    /// <returns>A task containing the optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    public static async Task<Optional<T>> WhereAsync<T>(this Task<Optional<T>> optionalTask, Func<T, bool> predicate) where T : notnull {
        return (await optionalTask).Where(predicate);
    }
    
    /// <summary>
    /// Filters the value based on an asynchronous predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optional">The optional to filter.</param>
    /// <param name="predicateAsync">The asynchronous predicate to test the value.</param>
    /// <returns>A task containing the optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    public static async Task<Optional<T>> WhereAsync<T>(this Optional<T> optional, Func<T, Task<bool>> predicateAsync) where T : notnull {
        return optional.HasValue && await predicateAsync(optional.Value) ? optional : Optional<T>.None();
    }
    
    /// <summary>
    /// Filters the value based on an asynchronous predicate, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optionalTask">The task containing the optional to filter.</param>
    /// <param name="predicateAsync">The asynchronous predicate to test the value.</param>
    /// <returns>A task containing the optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    public static async Task<Optional<T>> WhereAsync<T>(this Task<Optional<T>> optionalTask, Func<T, Task<bool>> predicateAsync) where T : notnull {
        return await (await optionalTask).WhereAsync(predicateAsync);
    }
    
    /// <summary>
    /// Matches on the optional value and returns a result, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The task containing the optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>A task containing the result of the executed function.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, TResult> onValue,
        Func<TResult> onNone) where T : notnull
    {
        return (await optionalTask).Match(onValue, onNone);
    }
    
    /// <summary>
    /// Matches on the optional value, applying an asynchronous function on value or a synchronous function on none.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValueAsync">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>A task containing the result of the executed function.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Optional<T> optional,
        Func<T, Task<TResult>> onValueAsync,
        Func<TResult> onNone) where T : notnull
    {
        return optional.HasValue ? await onValueAsync(optional.Value) : onNone();
    }
    
    /// <summary>
    /// Matches on the optional value, applying a synchronous function on value or an asynchronous function on none.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNoneAsync">The asynchronous function to execute if no value is present.</param>
    /// <returns>A task containing the result of the executed function.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Optional<T> optional,
        Func<T, TResult> onValue,
        Func<Task<TResult>> onNoneAsync) where T : notnull
    {
        return optional.HasValue ? onValue(optional.Value) : await onNoneAsync();
    }
    
    /// <summary>
    /// Matches on the optional value, applying an asynchronous function on value or a synchronous function on none, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The task containing the optional to match on.</param>
    /// <param name="onValueAsync">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>A task containing the result of the executed function.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, Task<TResult>> onValueAsync,
        Func<TResult> onNone) where T : notnull
    {
        return await (await optionalTask).MatchAsync(onValueAsync, onNone);
    }
    
    /// <summary>
    /// Matches on the optional value, applying a synchronous function on value or an asynchronous function on none, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The task containing the optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNoneAsync">The asynchronous function to execute if no value is present.</param>
    /// <returns>A task containing the result of the executed function.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, TResult> onValue,
        Func<Task<TResult>> onNoneAsync) where T : notnull
    {
        return await (await optionalTask).MatchAsync(onValue, onNoneAsync);
    }
    
    /// <summary>
    /// Matches on the optional value, applying asynchronous functions for both value and none cases.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValueAsync">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNoneAsync">The asynchronous function to execute if no value is present.</param>
    /// <returns>A task containing the result of the executed function.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Optional<T> optional,
        Func<T, Task<TResult>> onValueAsync,
        Func<Task<TResult>> onNoneAsync) where T : notnull
    {
        return optional.HasValue ? await onValueAsync(optional.Value) : await onNoneAsync();
    }
    
    /// <summary>
    /// Matches on the optional value, applying asynchronous functions for both value and none cases, awaiting the optional task.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The task containing the optional to match on.</param>
    /// <param name="onValueAsync">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNoneAsync">The asynchronous function to execute if no value is present.</param>
    /// <returns>A task containing the result of the executed function.</returns>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Optional<T>> optionalTask,
        Func<T, Task<TResult>> onValueAsync,
        Func<Task<TResult>> onNoneAsync) where T : notnull
    {
        return await (await optionalTask).MatchAsync(onValueAsync, onNoneAsync);
    }
}

