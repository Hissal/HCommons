using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Represents the result of an operation that can succeed with a value, fail with an error, or be cancelled.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public readonly record struct OperationResult<TValue>(
    OperationResultType Type,
    TValue? Value,
    Error Error,
    Cancelled Cancellation) 
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Type == OperationResultType.Success;

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => Type == OperationResultType.Failure;
    /// <summary>
    /// Gets a value indicating whether the operation was cancelled.
    /// </summary>
    public bool IsCancelled => Type == OperationResultType.Cancelled;

    /// <summary>
    /// Creates a successful operation result with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful operation result.</returns>
    [Pure]
    public static OperationResult<TValue> Success(TValue value) => new OperationResult<TValue>(OperationResultType.Success, value, Error.Empty, Monads.Cancelled.Empty);
    /// <summary>
    /// Creates a failed operation result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed operation result.</returns>
    [Pure]
    public static OperationResult<TValue> Failure(Error error) => new OperationResult<TValue>(OperationResultType.Failure, default, error, Monads.Cancelled.Empty);
    /// <summary>
    /// Creates a cancelled operation result with the specified cancellation.
    /// </summary>
    /// <param name="cancelled">The cancellation information.</param>
    /// <returns>A cancelled operation result.</returns>
    [Pure]
    public static OperationResult<TValue> Cancelled(Cancelled cancelled) => new OperationResult<TValue>(OperationResultType.Cancelled, default, Error.Empty, cancelled);

    /// <summary>
    /// Implicitly converts an error to a failed operation result.
    /// </summary>
    public static implicit operator OperationResult<TValue>(Error error) => Failure(error);
    /// <summary>
    /// Implicitly converts a cancellation to a cancelled operation result.
    /// </summary>
    public static implicit operator OperationResult<TValue>(Cancelled cancelled) => Cancelled(cancelled);
    
    /// <summary>
    /// Attempts to get the value if the operation succeeded.
    /// </summary>
    /// <param name="value">When this method returns, contains the value if the operation succeeded; otherwise, the default value.</param>
    /// <returns>True if the operation succeeded; otherwise, false.</returns>
    [Pure]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value) {
        value = Value;
        return IsSuccess;
    }
    
    /// <summary>
    /// Gets the value if the operation succeeded, otherwise returns the default value for the type.
    /// </summary>
    /// <returns>The value if the operation succeeded, otherwise default.</returns>
    [Pure]
    public TValue? GetValueOrDefault() => IsSuccess ? Value : default;
    /// <summary>
    /// Gets the value if the operation succeeded, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the operation failed or was cancelled.</param>
    /// <returns>The value if the operation succeeded, otherwise the specified default value.</returns>
    [Pure]
    public TValue GetValueOrDefault(TValue defaultValue) => IsSuccess ? Value : defaultValue;

    /// <summary>
    /// Returns a string representation of the operation result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded with the value, failed with the error, or was cancelled with the cancellation information.</returns>
    [Pure]
    public override string ToString() => Type switch {
        OperationResultType.Success => $"Success: {Value}",
        OperationResultType.Failure => $"Failure: {Error}",
        OperationResultType.Cancelled => $"Cancelled: {Cancellation}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}