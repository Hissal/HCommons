using System.Diagnostics.CodeAnalysis;

namespace HCommons.Monads;

/// <summary>
/// Represents an error with a message and optional exception.
/// </summary>
public readonly record struct Error(string Message, Exception? Exception = null) {
    /// <summary>
    /// Gets a value indicating whether this error has an associated exception.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool HasException => Exception is not null;
    
    /// <summary>
    /// Gets an empty error with no message or exception.
    /// </summary>
    public static Error Empty => new Error(string.Empty);
    
    /// <summary>
    /// Creates an error from an exception.
    /// </summary>
    /// <param name="ex">The exception.</param>
    /// <returns>An error containing the exception's message and the exception itself.</returns>
    public static Error FromException(Exception ex) => new Error(ex.Message, ex);
    /// <summary>
    /// Creates an error with the specified message.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <returns>An error containing the message.</returns>
    public static Error WithMessage(string message) => new Error(message);
    
    /// <summary>
    /// Implicitly converts an exception to an error.
    /// </summary>
    public static implicit operator Error(Exception exception) => FromException(exception);
    /// <summary>
    /// Implicitly converts a string message to an error.
    /// </summary>
    public static implicit operator Error(string message) => WithMessage(message);

    /// <summary>
    /// Returns a string representation of the error.
    /// </summary>
    /// <returns>A string containing the error message and exception if present.</returns>
    public override string ToString() => $"[Error]: {Message}{(HasException ? $" | -> [Exception]: {Exception}" : string.Empty)}";
}

/// <summary>
/// Represents an error with a message, optional exception, and an associated value.
/// </summary>
/// <typeparam name="TValue">The type of the associated value.</typeparam>
public readonly record struct Error<TValue>(TValue Value, string Message, Exception? Exception = null) {
    /// <summary>
    /// Gets a value indicating whether this error has an associated exception.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool HasException => Exception is not null;
    
    /// <summary>
    /// Creates an error from a value and exception.
    /// </summary>
    /// <param name="value">The associated value.</param>
    /// <param name="ex">The exception.</param>
    /// <returns>An error containing the value, the exception's message, and the exception itself.</returns>
    public static Error<TValue> FromException(TValue value, Exception ex) => new Error<TValue>(value, ex.Message, ex);
    /// <summary>
    /// Creates an error with a value and message.
    /// </summary>
    /// <param name="value">The associated value.</param>
    /// <param name="message">The error message.</param>
    /// <returns>An error containing the value and message.</returns>
    public static Error<TValue> WithMessage(TValue value, string message) => new Error<TValue>(value, message);
    /// <summary>
    /// Creates an error with only a value and no message.
    /// </summary>
    /// <param name="value">The associated value.</param>
    /// <returns>An error containing only the value.</returns>
    public static Error<TValue> ValueOnly(TValue value) => new Error<TValue>(value, string.Empty);
    
    /// <summary>
    /// Implicitly converts an error with value to an error without value.
    /// </summary>
    public static implicit operator Error(Error<TValue> error) => new Error(error.Message, error.Exception);
    
    /// <summary>
    /// Returns a string representation of the error.
    /// </summary>
    /// <returns>A string containing the error message, exception if present, and the associated value.</returns>
    public override string ToString() => $"[Error]: {Message}{(HasException ? $" | -> [Exception]: {Exception}" : string.Empty)} | -> [Value]: {Value}";
}

/// <summary>
/// Provides extension methods for Error types.
/// </summary>
public static class ErrorExtensions {
    /// <summary>
    /// Adds a value to an error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The error to add a value to.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>An error with the added value.</returns>
    public static Error<TValue> WithValue<TValue>(this Error error, TValue value) => new Error<TValue>(value, error.Message, error.Exception);
    /// <summary>
    /// Removes the value from an error.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The error to remove the value from.</param>
    /// <returns>An error without a value.</returns>
    public static Error WithoutValue<TValue>(this Error<TValue> error) => new Error(error.Message, error.Exception);
    
    /// <summary>
    /// Updates the message of an error.
    /// </summary>
    /// <param name="error">The error to update.</param>
    /// <param name="message">The new message.</param>
    /// <returns>An error with the updated message.</returns>
    public static Error WithMessage(this Error error, string message) => new Error(message, error.Exception);
    /// <summary>
    /// Updates the message of an error with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The error to update.</param>
    /// <param name="message">The new message.</param>
    /// <returns>An error with the updated message.</returns>
    public static Error<TValue> WithMessage<TValue>(this Error<TValue> error, string message) => new Error<TValue>(error.Value, message, error.Exception);
    
    /// <summary>
    /// Updates the exception of an error.
    /// </summary>
    /// <param name="error">The error to update.</param>
    /// <param name="exception">The new exception.</param>
    /// <returns>An error with the updated exception.</returns>
    public static Error WithException(this Error error, Exception exception) => new Error(error.Message, exception);
    /// <summary>
    /// Updates the exception of an error with a value.
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="error">The error to update.</param>
    /// <param name="exception">The new exception.</param>
    /// <returns>An error with the updated exception.</returns>
    public static Error<TValue> WithException<TValue>(this Error<TValue> error, Exception exception) => new Error<TValue>(error.Value, error.Message, exception);
}

