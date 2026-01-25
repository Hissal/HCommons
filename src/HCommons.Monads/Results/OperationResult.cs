using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Specifies the type of result for an operation.
/// </summary>
public enum OperationResultType : byte {
    /// <summary>
    /// The operation succeeded.
    /// </summary>
    Success,
    /// <summary>
    /// The operation failed.
    /// </summary>
    Failure,
    /// <summary>
    /// The operation was cancelled.
    /// </summary>
    Cancelled
}

/// <summary>
/// Represents the result of an operation that can succeed, fail with an error, or be cancelled.
/// </summary>
public readonly record struct OperationResult(OperationResultType Type, Error Error, Cancelled Cancellation) {
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
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
    /// Creates a successful operation result.
    /// </summary>
    /// <returns>A successful operation result.</returns>
    [Pure]
    public static OperationResult Success() => new OperationResult(OperationResultType.Success, Error.Empty, Monads.Cancelled.Empty);

    /// <summary>
    /// Creates a failed operation result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed operation result.</returns>
    [Pure]
    public static OperationResult Failure(Error error) => new OperationResult(OperationResultType.Failure, error, Monads.Cancelled.Empty);

    /// <summary>
    /// Creates a cancelled operation result with the specified cancellation.
    /// </summary>
    /// <param name="cancelled">The cancellation information.</param>
    /// <returns>A cancelled operation result.</returns>
    [Pure]
    public static OperationResult Cancelled(Cancelled cancelled) => new OperationResult(OperationResultType.Cancelled, Error.Empty, cancelled);

    /// <summary>
    /// Implicitly converts an error to a failed operation result.
    /// </summary>
    public static implicit operator OperationResult(Error error) => Failure(error);
    /// <summary>
    /// Implicitly converts a cancellation to a cancelled operation result.
    /// </summary>
    public static implicit operator OperationResult(Cancelled cancelled) => Cancelled(cancelled);

    /// <summary>
    /// Returns a string representation of the operation result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded, failed with the error, or was cancelled with the cancellation information.</returns>
    [Pure]
    public override string ToString() => Type switch {
        OperationResultType.Success => "Success",
        OperationResultType.Failure => $"Failure: {Error}",
        OperationResultType.Cancelled => $"Cancelled: {Cancellation}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}