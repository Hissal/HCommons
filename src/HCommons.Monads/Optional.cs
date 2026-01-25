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
    /// Returns a string representation of the optional.
    /// </summary>
    /// <returns>A string indicating whether the optional has a value and what that value is.</returns>
    [Pure]
    public override string ToString() => HasValue ? $"Some: {Value}" : "None";
}