
namespace HCommons.Monads;

/// <summary>
/// Represents a cancellation with a reason.
/// </summary>
public readonly record struct Cancelled(string Reason) {
    /// <summary>
    /// Gets an empty cancellation with no reason.
    /// </summary>
    public static Cancelled Empty => new Cancelled(string.Empty);
    
    /// <summary>
    /// Creates a cancellation with the specified reason.
    /// </summary>
    /// <param name="reason">The cancellation reason.</param>
    /// <returns>A cancellation with the specified reason.</returns>
    public static Cancelled Because(string reason) => new Cancelled(reason);
    
    /// <summary>
    /// Implicitly converts a string reason to a cancellation.
    /// </summary>
    public static implicit operator Cancelled(string reason) => new Cancelled(reason);
    /// <summary>
    /// Returns a string representation of the cancellation.
    /// </summary>
    /// <returns>A string containing the cancellation reason.</returns>
    public override string ToString() => $"[Cancelled]: {Reason}";
}

/// <summary>
/// Represents a cancellation with a reason and an associated value.
/// </summary>
/// <typeparam name="TValue">The type of the associated value.</typeparam>
public readonly record struct Cancelled<TValue>(TValue Value, string Reason) {
    /// <summary>
    /// Creates a cancellation with only a value and no reason.
    /// </summary>
    /// <param name="value">The associated value.</param>
    /// <returns>A cancellation containing only the value.</returns>
    public static Cancelled<TValue> ValueOnly(TValue value) => new Cancelled<TValue>(value, string.Empty); 
    /// <summary>
    /// Creates a cancellation with a value and reason.
    /// </summary>
    /// <param name="value">The associated value.</param>
    /// <param name="reason">The cancellation reason.</param>
    /// <returns>A cancellation containing the value and reason.</returns>
    public static Cancelled<TValue> Because(TValue value, string reason) => new Cancelled<TValue>(value, reason);
    
    /// <summary>
    /// Implicitly converts a cancellation with value to a cancellation without value.
    /// </summary>
    public static implicit operator Cancelled(Cancelled<TValue> cancelled) => new Cancelled(cancelled.Reason);
    /// <summary>
    /// Returns a string representation of the cancellation.
    /// </summary>
    /// <returns>A string containing the cancellation reason and associated value.</returns>
    public override string ToString() => $"[Cancelled]: {Reason}, [Value]: {Value}";
}

/// <summary>
/// Provides extension methods for Cancelled types.
/// </summary>
public static class CancelledExtensions {
    /// <summary>
    /// Adds a value to a cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="cancelled">The cancellation to add a value to.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>A cancellation with the added value.</returns>
    public static Cancelled<TValue> WithValue<TValue>(this Cancelled cancelled, TValue value) => new Cancelled<TValue>(value, cancelled.Reason);
    /// <summary>
    /// Removes the value from a cancellation.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="cancelled">The cancellation to remove the value from.</param>
    /// <returns>A cancellation without a value.</returns>
    public static Cancelled WithoutValue<TValue>(this Cancelled<TValue> cancelled) => new Cancelled(cancelled.Reason);
}
