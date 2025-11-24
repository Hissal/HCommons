using System.Diagnostics.Contracts;
using Cysharp.Threading.Tasks;
using HCommons.Monads;

namespace HCommons.UniTask.Monads;

/// <summary>
/// Provides asynchronous extension methods for Optional{T} types.
/// </summary>
public static class OptionalExtensionsAsyncUniTask {
    /// <summary>
    /// Projects the value into a new form using the specified selector function, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to transform.</param>
    /// <param name="selector">The function to apply to the value if present.</param>
    /// <returns>A UniTask containing an optional with the transformed value if present, or an empty optional if no value is present.</returns>
    [Pure]
    public static async UniTask<Optional<TResult>> SelectAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask, 
        Func<T, TResult> selector) 
        where T : notnull 
        where TResult : notnull 
    {
        return (await optionalTask).Select(selector);
    }

    /// <summary>
    /// Projects the value into a new form using the specified asynchronous selector function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to transform.</param>
    /// <param name="selector">The asynchronous function to apply to the value if present.</param>
    /// <returns>A UniTask containing an optional with the transformed value if present, or an empty optional if no value is present.</returns>
    [Pure]
    public static async UniTask<Optional<TResult>> SelectAsyncUniTask<T, TResult>(
        this Optional<T> optional,
        Func<T, UniTask<TResult>> selector) 
        where T : notnull 
        where TResult : notnull 
    {
        return optional.HasValue ? Optional<TResult>.Some(await selector(optional.Value)) : Optional<TResult>.None();
    }

    /// <summary>
    /// Projects the value into a new form using the specified asynchronous selector function, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to transform.</param>
    /// <param name="selector">The asynchronous function to apply to the value if present.</param>
    /// <returns>A UniTask containing an optional with the transformed value if present, or an empty optional if no value is present.</returns>
    [Pure]
    public static async UniTask<Optional<TResult>> SelectAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask, 
        Func<T, UniTask<TResult>> selector)
        where T : notnull 
        where TResult : notnull 
    {
        return await (await optionalTask).SelectAsyncUniTask(selector);
    }

    /// <summary>
    /// Projects the value into a new optional using the specified binder function, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to bind.</param>
    /// <param name="binder">The function to apply to the value if present.</param>
    /// <returns>A UniTask containing the result of applying the binder, or an empty optional if no value is present.</returns>
    [Pure]
    public static async UniTask<Optional<TResult>> BindAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask,
        Func<T, Optional<TResult>> binder)
        where T : notnull 
        where TResult : notnull 
    {
        return (await optionalTask).Bind(binder);
    }

    /// <summary>
    /// Projects the value into a new optional using the specified asynchronous binder function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to bind.</param>
    /// <param name="binder">The asynchronous function to apply to the value if present.</param>
    /// <returns>A UniTask containing the result of applying the binder, or an empty optional if no value is present.</returns>
    [Pure]
    public static async UniTask<Optional<TResult>> BindAsyncUniTask<T, TResult>(
        this Optional<T> optional,
        Func<T, UniTask<Optional<TResult>>> binder) 
        where T : notnull 
        where TResult : notnull 
    {
        return optional.HasValue ? await binder(optional.Value) : Optional<TResult>.None();
    }

    /// <summary>
    /// Projects the value into a new optional using the specified asynchronous binder function, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to bind.</param>
    /// <param name="binder">The asynchronous function to apply to the value if present.</param>
    /// <returns>A UniTask containing the result of applying the binder, or an empty optional if no value is present.</returns>
    [Pure]
    public static async UniTask<Optional<TResult>> BindAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask,
        Func<T, UniTask<Optional<TResult>>> binder)
        where T : notnull
        where TResult : notnull 
    {
        return await (await optionalTask).BindAsyncUniTask(binder);
    }

    /// <summary>
    /// Filters the value based on a predicate, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to filter.</param>
    /// <param name="predicate">The predicate to test the value.</param>
    /// <returns>A UniTask containing the optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    [Pure]
    public static async UniTask<Optional<T>> WhereAsyncUniTask<T>(this UniTask<Optional<T>> optionalTask,
        Func<T, bool> predicate) where T : notnull {
        return (await optionalTask).Where(predicate);
    }

    /// <summary>
    /// Filters the value based on an asynchronous predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optional">The optional to filter.</param>
    /// <param name="predicate">The asynchronous predicate to test the value.</param>
    /// <returns>A UniTask containing the optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    [Pure]
    public static async UniTask<Optional<T>> WhereAsyncUniTask<T>(this Optional<T> optional,
        Func<T, UniTask<bool>> predicate) where T : notnull {
        return optional.HasValue && await predicate(optional.Value) ? optional : Optional<T>.None();
    }

    /// <summary>
    /// Filters the value based on an asynchronous predicate, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to filter.</param>
    /// <param name="predicate">The asynchronous predicate to test the value.</param>
    /// <returns>A UniTask containing the optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    [Pure]
    public static async UniTask<Optional<T>> WhereAsyncUniTask<T>(this UniTask<Optional<T>> optionalTask,
        Func<T, UniTask<bool>> predicate) where T : notnull {
        return await (await optionalTask).WhereAsyncUniTask(predicate);
    }

    /// <summary>
    /// Matches on the optional value and returns a result, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>A UniTask containing the result of the executed function.</returns>
    [Pure]
    public static async UniTask<TResult> MatchAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask,
        Func<T, TResult> onValue,
        Func<TResult> onNone) where T : notnull {
        return (await optionalTask).Match(onValue, onNone);
    }

    /// <summary>
    /// Matches on the optional value, applying an asynchronous function on value or a synchronous function on none.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValue">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>A UniTask containing the result of the executed function.</returns>
    [Pure]
    public static async UniTask<TResult> MatchAsyncUniTask<T, TResult>(
        this Optional<T> optional,
        Func<T, UniTask<TResult>> onValue,
        Func<TResult> onNone) where T : notnull {
        return optional.HasValue ? await onValue(optional.Value) : onNone();
    }

    /// <summary>
    /// Matches on the optional value, applying a synchronous function on value or an asynchronous function on none.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The asynchronous function to execute if no value is present.</param>
    /// <returns>A UniTask containing the result of the executed function.</returns>
    [Pure]
    public static async UniTask<TResult> MatchAsyncUniTask<T, TResult>(
        this Optional<T> optional,
        Func<T, TResult> onValue,
        Func<UniTask<TResult>> onNone) where T : notnull {
        return optional.HasValue ? onValue(optional.Value) : await onNone();
    }

    /// <summary>
    /// Matches on the optional value, applying an asynchronous function on value or a synchronous function on none, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to match on.</param>
    /// <param name="onValue">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>A UniTask containing the result of the executed function.</returns>
    [Pure]
    public static async UniTask<TResult> MatchAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask,
        Func<T, UniTask<TResult>> onValue,
        Func<TResult> onNone) where T : notnull {
        return await (await optionalTask).MatchAsyncUniTask(onValue, onNone);
    }

    /// <summary>
    /// Matches on the optional value, applying a synchronous function on value or an asynchronous function on none, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The asynchronous function to execute if no value is present.</param>
    /// <returns>A UniTask containing the result of the executed function.</returns>
    [Pure]
    public static async UniTask<TResult> MatchAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask,
        Func<T, TResult> onValue,
        Func<UniTask<TResult>> onNone) where T : notnull {
        return await (await optionalTask).MatchAsyncUniTask(onValue, onNone);
    }

    /// <summary>
    /// Matches on the optional value, applying asynchronous functions for both value and none cases.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValue">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNone">The asynchronous function to execute if no value is present.</param>
    /// <returns>A UniTask containing the result of the executed function.</returns>
    [Pure]
    public static async UniTask<TResult> MatchAsyncUniTask<T, TResult>(
        this Optional<T> optional,
        Func<T, UniTask<TResult>> onValue,
        Func<UniTask<TResult>> onNone) where T : notnull {
        return optional.HasValue ? await onValue(optional.Value) : await onNone();
    }

    /// <summary>
    /// Matches on the optional value, applying asynchronous functions for both value and none cases, awaiting the optional UniTask.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optionalTask">The UniTask containing the optional to match on.</param>
    /// <param name="onValue">The asynchronous function to execute if a value is present.</param>
    /// <param name="onNone">The asynchronous function to execute if no value is present.</param>
    /// <returns>A UniTask containing the result of the executed function.</returns>
    [Pure]
    public static async UniTask<TResult> MatchAsyncUniTask<T, TResult>(
        this UniTask<Optional<T>> optionalTask,
        Func<T, UniTask<TResult>> onValue,
        Func<UniTask<TResult>> onNone) where T : notnull {
        return await (await optionalTask).MatchAsyncUniTask(onValue, onNone);
    }
}