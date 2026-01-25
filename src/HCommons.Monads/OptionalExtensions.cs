using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Provides extension methods for Optional{T} types.
/// </summary>
public static class OptionalExtensions {
    /// <summary>
    /// Projects the value into a new form using the specified selector function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to transform.</param>
    /// <param name="selector">The function to apply to the value.</param>
    /// <returns>An optional containing the transformed value, or an empty optional if no value is present.</returns>
    [Pure]
    public static Optional<TResult> Select<T, TResult>(this Optional<T> optional, Func<T, TResult> selector) where T : notnull where TResult : notnull =>
        optional.HasValue ? Optional<TResult>.Some(selector(optional.Value)) : Optional<TResult>.None();
    
    /// <summary>
    /// Projects the value into a new form with state using the specified selector function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to transform.</param>
    /// <param name="state">The state to pass to the selector.</param>
    /// <param name="selector">The function to apply to the state and value.</param>
    /// <returns>An optional containing the transformed value, or an empty optional if no value is present.</returns>
    [Pure]
    public static Optional<TResult> Select<T, TState, TResult>(this Optional<T> optional, TState state, Func<TState, T, TResult> selector) where T : notnull where TResult : notnull =>
        optional.HasValue ? Optional<TResult>.Some(selector(state, optional.Value)) : Optional<TResult>.None();
    
    /// <summary>
    /// Projects the value into a new optional using the specified selector function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to bind.</param>
    /// <param name="binder">The function to apply to the value.</param>
    /// <returns>The result of applying the binder, or an empty optional if no value is present.</returns>
    [Pure]
    public static Optional<TResult> Bind<T, TResult>(this Optional<T> optional, Func<T, Optional<TResult>> binder) where T : notnull where TResult : notnull =>
        optional.HasValue ? binder(optional.Value) : Optional<TResult>.None();
    
    /// <summary>
    /// Projects the value into a new optional with state using the specified selector function.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to bind.</param>
    /// <param name="state">The state to pass to the binder.</param>
    /// <param name="binder">The function to apply to the state and value.</param>
    /// <returns>The result of applying the binder, or an empty optional if no value is present.</returns>
    [Pure]
    public static Optional<TResult> Bind<T, TState, TResult>(this Optional<T> optional, TState state, Func<TState, T, Optional<TResult>> binder) where T : notnull where TResult : notnull =>
        optional.HasValue ? binder(state, optional.Value) : Optional<TResult>.None();
    
    /// <summary>
    /// Projects the value into an intermediate optional, then projects both values into a final result.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TInner">The type of the intermediate value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to transform.</param>
    /// <param name="innerSelector">The function to project the value into an intermediate optional.</param>
    /// <param name="resultSelector">The function to project both values into a final result.</param>
    /// <returns>An optional containing the final result, or an empty optional if either projection fails.</returns>
    [Pure]
    public static Optional<TResult> SelectMany<T, TInner, TResult>(
        this Optional<T> optional,
        Func<T, Optional<TInner>> innerSelector,
        Func<T, TInner, TResult> resultSelector) 
        where T : notnull 
        where TInner : notnull 
        where TResult : notnull
    {
        if (!optional.HasValue) 
            return Optional<TResult>.None();
        
        var inner = innerSelector(optional.Value);
        return inner.HasValue
            ? Optional<TResult>.Some(resultSelector(optional.Value, inner.Value))
            : Optional<TResult>.None();
    }
    
    /// <summary>
    /// Projects the value into an intermediate optional with state, then projects both values into a final result.
    /// </summary>
    /// <typeparam name="T">The type of the source value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TInner">The type of the intermediate value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="optional">The optional to transform.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="innerSelector">The function to project the state and value into an intermediate optional.</param>
    /// <param name="resultSelector">The function to project the state and both values into a final result.</param>
    /// <returns>An optional containing the final result, or an empty optional if either projection fails.</returns>
    [Pure]
    public static Optional<TResult> SelectMany<T, TState, TInner, TResult>(
        this Optional<T> optional,
        TState state,
        Func<TState, T, Optional<TInner>> innerSelector,
        Func<TState, T, TInner, TResult> resultSelector) 
        where T : notnull 
        where TInner : notnull 
        where TResult : notnull
    {
        if (!optional.HasValue) 
            return Optional<TResult>.None();
        
        var inner = innerSelector(state, optional.Value);
        return inner.HasValue
            ? Optional<TResult>.Some(resultSelector(state, optional.Value, inner.Value))
            : Optional<TResult>.None();
    }
    
    /// <summary>
    /// Filters the value based on a predicate.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optional">The optional to filter.</param>
    /// <param name="predicate">The predicate to test the value.</param>
    /// <returns>The optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    [Pure]
    public static Optional<T> Where<T>(this Optional<T> optional, Func<T, bool> predicate) where T : notnull =>
        optional.HasValue && predicate(optional.Value) ? optional : Optional<T>.None();
    
    /// <summary>
    /// Filters the value based on a predicate with state.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="optional">The optional to filter.</param>
    /// <param name="state">The state to pass to the predicate.</param>
    /// <param name="predicate">The predicate to test the state and value.</param>
    /// <returns>The optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    [Pure]
    public static Optional<T> Where<T, TState>(this Optional<T> optional, TState state, Func<TState, T, bool> predicate) where T : notnull =>
        optional.HasValue && predicate(state, optional.Value) ? optional : Optional<T>.None();
    
    /// <summary>
    /// Matches on the optional value and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public static TResult Match<T, TResult>(this Optional<T> optional, Func<T, TResult> onValue, Func<TResult> onNone) where T : notnull =>
        optional.HasValue ? onValue(optional.Value) : onNone();
    
    /// <summary>
    /// Matches on the optional value and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The result to return if no value is present.</param>
    /// <returns>The result of the executed function or the specified result.</returns>
    [Pure]
    public static TResult Match<T, TResult>(this Optional<T> optional, Func<T, TResult> onValue, TResult onNone) where T : notnull =>
        optional.HasValue ? onValue(optional.Value) : onNone;
    
    /// <summary>
    /// Matches on the optional value with state and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public static TResult Match<T, TState, TResult>(this Optional<T> optional, TState state, Func<TState, T, TResult> onValue, Func<TState, TResult> onNone) where T : notnull =>
        optional.HasValue ? onValue(state, optional.Value) : onNone(state);
    
    /// <summary>
    /// Matches on the optional value with state and returns a result.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="optional">The optional to match on.</param>
    /// <param name="state">The state to pass to the function.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The result to return if no value is present.</param>
    /// <returns>The result of the executed function or the specified result.</returns>
    [Pure]
    public static TResult Match<T, TState, TResult>(this Optional<T> optional, TState state, Func<TState, T, TResult> onValue, TResult onNone) where T : notnull =>
        optional.HasValue ? onValue(state, optional.Value) : onNone;
    
    /// <summary>
    /// Executes an action based on whether a value is present.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="optional">The optional to switch on.</param>
    /// <param name="onValue">The action to execute if a value is present.</param>
    /// <param name="onNone">The action to execute if no value is present.</param>
    public static void Switch<T>(this Optional<T> optional, Action<T> onValue, Action onNone) where T : notnull {
        if (optional.HasValue) onValue(optional.Value);
        else onNone();
    }
    
    /// <summary>
    /// Executes an action with state based on whether a value is present.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="optional">The optional to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onValue">The action to execute if a value is present.</param>
    /// <param name="onNone">The action to execute if no value is present.</param>
    public static void Switch<T, TState>(this Optional<T> optional, TState state, Action<TState, T> onValue, Action<TState> onNone) where T : notnull {
        if (optional.HasValue) onValue(state, optional.Value);
        else onNone(state);
    }
}
