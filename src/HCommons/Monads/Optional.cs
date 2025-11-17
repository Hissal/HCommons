using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace HCommons.Monads;

/// <summary>
/// Represents an optional value that may or may not be present.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public readonly record struct Optional<T> where T : notnull {
    /// <summary>
    /// Gets a value indicating whether the optional contains a value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    /// <summary>
    /// Gets the value if present, otherwise null.
    /// </summary>
    public T? Value { get; }

    Optional(bool hasValue, T? value = default) {
        HasValue = hasValue;
        Value = value;
    }

    /// <summary>
    /// Implicitly converts a value to an optional containing that value.
    /// </summary>
    public static implicit operator Optional<T>(T value) => Some(value);

    /// <summary>
    /// Creates an optional containing the specified value.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <returns>An optional containing the value.</returns>
    [Pure]
    public static Optional<T> Some(T value) => new Optional<T>(true, value);

    /// <summary>
    /// Creates an empty optional with no value.
    /// </summary>
    /// <returns>An optional with no value.</returns>
    [Pure]
    public static Optional<T> None() => new Optional<T>(false);

    /// <summary>
    /// Attempts to get the value if present.
    /// </summary>
    /// <param name="value">When this method returns, contains the value if present; otherwise, the default value.</param>
    /// <returns>True if the optional contains a value; otherwise, false.</returns>
    [Pure]
    public bool TryGetValue([NotNullWhen(true)] out T? value) {
        value = HasValue ? Value : default;
        return HasValue;
    }

    /// <summary>
    /// Gets the value if present, otherwise returns the default value for the type.
    /// </summary>
    /// <returns>The value if present, otherwise default.</returns>
    [Pure]
    public T? GetValueOrDefault() => HasValue ? Value : default;

    /// <summary>
    /// Gets the value if present, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if no value is present.</param>
    /// <returns>The value if present, otherwise the specified default value.</returns>
    [Pure]
    public T GetValueOrDefault(T defaultValue) => HasValue ? Value : defaultValue;

    /// <summary>
    /// Matches on the optional value and returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public TResult Match<TResult>(Func<T, TResult> onValue, Func<TResult> onNone) =>
        HasValue ? onValue(Value) : onNone();

    /// <summary>
    /// Matches on the optional value and returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The result to return if no value is present.</param>
    /// <returns>The result of the executed function or the specified result.</returns>
    [Pure]
    public TResult Match<TResult>(Func<T, TResult> onValue, TResult onNone) =>
        HasValue ? onValue(Value) : onNone;

    /// <summary>
    /// Matches on the optional value with state and returns a result.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The function to execute if no value is present.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public TResult Match<TState, TResult>(TState state, Func<TState, T, TResult> onValue,
        Func<TState, TResult> onNone) =>
        HasValue ? onValue(state, Value) : onNone(state);

    /// <summary>
    /// Matches on the optional value with state and returns a result.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="state">The state to pass to the function.</param>
    /// <param name="onValue">The function to execute if a value is present.</param>
    /// <param name="onNone">The result to return if no value is present.</param>
    /// <returns>The result of the executed function or the specified result.</returns>
    [Pure]
    public TResult Match<TState, TResult>(TState state, Func<TState, T, TResult> onValue, TResult onNone) =>
        HasValue ? onValue(state, Value) : onNone;

    /// <summary>
    /// Projects the value into a new optional using the specified selector function.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="selector">The function to apply to the value.</param>
    /// <returns>The result of applying the selector, or an empty optional if no value is present.</returns>
    [Pure]
    public Optional<TResult> Bind<TResult>(Func<T, Optional<TResult>> selector) where TResult : notnull =>
        HasValue ? selector(Value) : Optional<TResult>.None();

    /// <summary>
    /// Projects the value into a new optional with state using the specified selector function.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="state">The state to pass to the selector.</param>
    /// <param name="selector">The function to apply to the state and value.</param>
    /// <returns>The result of applying the selector, or an empty optional if no value is present.</returns>
    [Pure]
    public Optional<TResult> Bind<TState, TResult>(TState state, Func<TState, T, Optional<TResult>> selector)
        where TResult : notnull =>
        HasValue ? selector(state, Value) : Optional<TResult>.None();
    
    /// <summary>
    /// Projects the value into a new form using the specified selector function.
    /// </summary>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="selector">The function to apply to the value.</param>
    /// <returns>An optional containing the transformed value, or an empty optional if no value is present.</returns>
    [Pure]
    public Optional<TResult> Select<TResult>(Func<T, TResult> selector) where TResult : notnull =>
        HasValue ? Optional<TResult>.Some(selector(Value)) : Optional<TResult>.None();
    
    /// <summary>
    /// Projects the value into a new form with state using the specified selector function.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="state">The state to pass to the selector.</param>
    /// <param name="selector">The function to apply to the state and value.</param>
    /// <returns>An optional containing the transformed value, or an empty optional if no value is present.</returns>
    [Pure]
    public Optional<TResult> Select<TState, TResult>(TState state, Func<TState, T, TResult> selector) where TResult : notnull =>
        HasValue ? Optional<TResult>.Some(selector(state, Value)) : Optional<TResult>.None();
    
    /// <summary>
    /// Projects the value into an intermediate optional, then projects both values into a final result.
    /// </summary>
    /// <typeparam name="TInner">The type of the intermediate value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="innerSelector">The function to project the value into an intermediate optional.</param>
    /// <param name="resultSelector">The function to project both values into a final result.</param>
    /// <returns>An optional containing the final result, or an empty optional if either projection fails.</returns>
    [Pure]
    public Optional<TResult> SelectMany<TInner, TResult>(
        Func<T, Optional<TInner>> innerSelector,
        Func<T, TInner, TResult> resultSelector) 
        where TInner : notnull 
        where TResult : notnull
    {
        if (!HasValue) 
            return Optional<TResult>.None();
        
        var inner = innerSelector(Value);
        return inner.HasValue
            ? Optional<TResult>.Some(resultSelector(Value, inner.Value))
            : Optional<TResult>.None();
    }
    
    /// <summary>
    /// Projects the value into an intermediate optional with state, then projects both values into a final result.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TInner">The type of the intermediate value.</typeparam>
    /// <typeparam name="TResult">The type of the result value.</typeparam>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="innerSelector">The function to project the state and value into an intermediate optional.</param>
    /// <param name="resultSelector">The function to project the state and both values into a final result.</param>
    /// <returns>An optional containing the final result, or an empty optional if either projection fails.</returns>
    [Pure]
    public Optional<TResult> SelectMany<TState, TInner, TResult>(
        TState state,
        Func<TState, T, Optional<TInner>> innerSelector,
        Func<TState, T, TInner, TResult> resultSelector) 
        where TInner : notnull 
        where TResult : notnull
    {
        if (!HasValue) 
            return Optional<TResult>.None();
        
        var inner = innerSelector(state, Value);
        return inner.HasValue
            ? Optional<TResult>.Some(resultSelector(state, Value, inner.Value))
            : Optional<TResult>.None();
    }
    
    /// <summary>
    /// Filters the value based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to test the value.</param>
    /// <returns>The optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    [Pure]
    public Optional<T> Where(Func<T, bool> predicate) =>
        HasValue && predicate(Value) ? this : None();
    
    /// <summary>
    /// Filters the value based on a predicate with state.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state to pass to the predicate.</param>
    /// <param name="predicate">The predicate to test the state and value.</param>
    /// <returns>The optional if the value satisfies the predicate; otherwise, an empty optional.</returns>
    [Pure]
    public Optional<T> Where<TState>(TState state, Func<TState, T, bool> predicate) =>
        HasValue && predicate(state, Value) ? this : None();

    /// <summary>
    /// Executes an action based on whether a value is present.
    /// </summary>
    /// <param name="onValue">The action to execute if a value is present.</param>
    /// <param name="onNone">The action to execute if no value is present.</param>
    public void Switch(Action<T> onValue, Action onNone) {
        if (HasValue) onValue(Value);
        else onNone();
    }

    /// <summary>
    /// Executes an action with state based on whether a value is present.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onValue">The action to execute if a value is present.</param>
    /// <param name="onNone">The action to execute if no value is present.</param>
    public void Switch<TState>(TState state, Action<TState, T> onValue, Action<TState> onNone) {
        if (HasValue) onValue(state, Value);
        else onNone(state);
    }

    /// <summary>
    /// Returns a string representation of the optional.
    /// </summary>
    /// <returns>A string indicating whether the optional has a value and what that value is.</returns>
    [Pure]
    public override string ToString() => HasValue ? $"Some: {Value}" : "None";
}