using System.Diagnostics.Contracts;
using System.Diagnostics.CodeAnalysis;

namespace HCommons.Monads;

public readonly record struct Optional<T> where T : notnull {
    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue { get; }

    public T? Value { get; }

    Optional(bool hasValue, T? value = default) {
        HasValue = hasValue;
        Value = value;
    }

    public static implicit operator Optional<T>(T value) => Some(value);

    [Pure]
    public static Optional<T> Some(T value) => new Optional<T>(true, value);

    [Pure]
    public static Optional<T> None() => new Optional<T>(false);

    [Pure]
    public bool TryGetValue([NotNullWhen(true)] out T? value) {
        value = HasValue ? Value : default;
        return HasValue;
    }

    [Pure]
    public T? GetValueOrDefault() => HasValue ? Value : default;

    [Pure]
    public T GetValueOrDefault(T defaultValue) => HasValue ? Value! : defaultValue;

    [Pure]
    public TResult Match<TResult>(Func<T, TResult> onValue, Func<TResult> onNone) =>
        HasValue ? onValue(Value!) : onNone();

    [Pure]
    public TResult Match<TResult>(Func<T, TResult> onValue, TResult onNone) =>
        HasValue ? onValue(Value!) : onNone;

    [Pure]
    public TResult Match<TState, TResult>(TState state, Func<TState, T, TResult> onValue,
        Func<TState, TResult> onNone) =>
        HasValue ? onValue(state, Value!) : onNone(state);

    [Pure]
    public TResult Match<TState, TResult>(TState state, Func<TState, T, TResult> onValue, TResult onNone) =>
        HasValue ? onValue(state, Value!) : onNone;

    [Pure]
    public Optional<TResult> Select<TResult>(Func<T, Optional<TResult>> selector) where TResult : notnull =>
        HasValue ? selector(Value!) : Optional<TResult>.None();

    [Pure]
    public Optional<TResult> Select<TState, TResult>(TState state, Func<TState, T, Optional<TResult>> selector)
        where TResult : notnull =>
        HasValue ? selector(state, Value!) : Optional<TResult>.None();

    public void Switch(Action<T> onValue, Action onNone) {
        if (HasValue) onValue(Value!);
        else onNone();
    }

    public void Switch<TState>(TState state, Action<TState, T> onValue, Action<TState> onNone) {
        if (HasValue) onValue(state, Value!);
        else onNone(state);
    }

    [Pure]
    public override string ToString() => HasValue ? $"Some: {Value}" : "None";
}