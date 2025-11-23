using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Represents the result of an operation that can either succeed with a success value or fail with a failure value.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
/// <typeparam name="TFailure">The type of the failure value.</typeparam>
public readonly record struct Result<TSuccess, TFailure> where TSuccess : notnull where TFailure : notnull {
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(FailureValue))]
    public bool IsSuccess { get; }
    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(FailureValue))]
    public bool IsFailure => !IsSuccess;
    
    /// <summary>
    /// Gets the success value if the operation succeeded, otherwise null.
    /// </summary>
    public TSuccess? Value { get; }
    /// <summary>
    /// Gets the failure value if the operation failed, otherwise null.
    /// </summary>
    public TFailure? FailureValue { get; }
        
    Result(bool isSuccess, TSuccess? value = default, TFailure? failureValue = default) {
        IsSuccess = isSuccess;
        Value = value;
        FailureValue = failureValue;
    }
        
    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful result.</returns>
    [Pure]
    public static Result<TSuccess, TFailure> Success(TSuccess value) => new (true, value);
    /// <summary>
    /// Creates a failed result with the specified failure value.
    /// </summary>
    /// <param name="value">The failure value.</param>
    /// <returns>A failed result.</returns>
    [Pure]
    public static Result<TSuccess, TFailure> Failure(TFailure value) => new (false, default, value);
        
    /// <summary>
    /// Implicitly converts a success value to a successful result.
    /// </summary>
    public static implicit operator Result<TSuccess, TFailure>(TSuccess value) => Success(value);
    /// <summary>
    /// Implicitly converts a failure value to a failed result.
    /// </summary>
    public static implicit operator Result<TSuccess, TFailure>(TFailure value) => Failure(value);
    
    /// <summary>
    /// Attempts to get the success value if the operation succeeded.
    /// </summary>
    /// <param name="value">When this method returns, contains the success value if the operation succeeded; otherwise, the default value.</param>
    /// <returns>True if the operation succeeded; otherwise, false.</returns>
    [Pure]
    public bool TryGetSuccess([NotNullWhen(true)] out TSuccess? value) {
        value = Value;
        return IsSuccess;
    }
    
    /// <summary>
    /// Attempts to get the failure value if the operation failed.
    /// </summary>
    /// <param name="value">When this method returns, contains the failure value if the operation failed; otherwise, the default value.</param>
    /// <returns>True if the operation failed; otherwise, false.</returns>
    [Pure]
    public bool TryGetFailure([NotNullWhen(true)] out TFailure? value) {
        value = FailureValue;
        return IsFailure;
    }
    
    /// <summary>
    /// Gets the success value if the operation succeeded, otherwise returns the default value for the type.
    /// </summary>
    /// <returns>The success value if the operation succeeded, otherwise default.</returns>
    [Pure]
    public TSuccess? GetSuccessOrDefault() => IsSuccess ? Value : default;
    /// <summary>
    /// Gets the success value if the operation succeeded, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the operation failed.</param>
    /// <returns>The success value if the operation succeeded, otherwise the specified default value.</returns>
    [Pure]
    public TSuccess GetSuccessOrDefault(TSuccess defaultValue) => IsSuccess ? Value : defaultValue;
    
    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded with the success value or failed with the failure value.</returns>
    [Pure]
    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {FailureValue}";
}