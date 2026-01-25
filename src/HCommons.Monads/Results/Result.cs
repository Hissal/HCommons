using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Represents the result of an operation that can either succeed or fail with an error.
/// </summary>
public readonly record struct Result(bool IsSuccess, Error Error) {
    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A successful result.</returns>
    [Pure]
    public static Result Success() => new Result(true, Error.Empty);
    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result.</returns>
    [Pure]
    public static Result Failure(Error error) => new Result(false, error);

    /// <summary>
    /// Implicitly converts an error to a failed result.
    /// </summary>
    public static implicit operator Result(Error error) => Failure(error);

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded or failed with the error.</returns>
    [Pure]
    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}