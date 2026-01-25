using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Represents the result of an operation that can succeed with a success value, fail with a failure value, or be cancelled with a cancellation value.
/// </summary>
/// <typeparam name="TSuccess">The type of the success value.</typeparam>
/// <typeparam name="TFailure">The type of the failure value.</typeparam>
/// <typeparam name="TCancelled">The type of the cancellation value.</typeparam>
public readonly record struct OperationResult<TSuccess, TFailure, TCancelled>(
    OperationResultType Type,
    TSuccess? SuccessValue = default,
    TFailure? FailureValue = default, 
    TCancelled? CancelledValue = default
    )
    where TSuccess : notnull 
    where TFailure : notnull 
    where TCancelled : notnull 
{
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    [MemberNotNullWhen(true, nameof(SuccessValue))]
    public bool IsSuccess => Type == OperationResultType.Success;
    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    [MemberNotNullWhen(true, nameof(FailureValue))]
    public bool IsFailure => Type == OperationResultType.Failure;
    /// <summary>
    /// Gets a value indicating whether the operation was cancelled.
    /// </summary>
    [MemberNotNullWhen(true, nameof(CancelledValue))]
    public bool IsCancelled => Type == OperationResultType.Cancelled;
    
    /// <summary>
    /// Implicitly converts a success value to a successful operation result.
    /// </summary>
    public static implicit operator OperationResult<TSuccess, TFailure, TCancelled>(TSuccess value) => Success(value);
    /// <summary>
    /// Implicitly converts a failure value to a failed operation result.
    /// </summary>
    public static implicit operator OperationResult<TSuccess, TFailure, TCancelled>(TFailure failure) => Failure(failure);
    /// <summary>
    /// Implicitly converts a cancellation value to a cancelled operation result.
    /// </summary>
    public static implicit operator OperationResult<TSuccess, TFailure, TCancelled>(TCancelled cancelled) => Cancelled(cancelled);
    
    /// <summary>
    /// Creates a successful operation result with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful operation result.</returns>
    [Pure]
    public static OperationResult<TSuccess, TFailure, TCancelled> Success(TSuccess value) => new (OperationResultType.Success, SuccessValue: value);
    /// <summary>
    /// Creates a failed operation result with the specified failure value.
    /// </summary>
    /// <param name="value">The failure value.</param>
    /// <returns>A failed operation result.</returns>
    [Pure]
    public static OperationResult<TSuccess, TFailure, TCancelled> Failure(TFailure value) => new (OperationResultType.Failure, FailureValue: value);
    /// <summary>
    /// Creates a cancelled operation result with the specified cancellation value.
    /// </summary>
    /// <param name="value">The cancellation value.</param>
    /// <returns>A cancelled operation result.</returns>
    [Pure]
    public static OperationResult<TSuccess, TFailure, TCancelled> Cancelled(TCancelled value) => new (OperationResultType.Cancelled, CancelledValue: value);
        
    /// <summary>
    /// Attempts to get the success value if the operation succeeded.
    /// </summary>
    /// <param name="value">When this method returns, contains the success value if the operation succeeded; otherwise, the default value.</param>
    /// <returns>True if the operation succeeded; otherwise, false.</returns>
    [Pure]
    public bool TryGetSuccess([NotNullWhen(true)] out TSuccess? value) {
        value = SuccessValue;
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
    /// Attempts to get the cancellation value if the operation was cancelled.
    /// </summary>
    /// <param name="value">When this method returns, contains the cancellation value if the operation was cancelled; otherwise, the default value.</param>
    /// <returns>True if the operation was cancelled; otherwise, false.</returns>
    [Pure]
    public bool TryGetCancelled([NotNullWhen(true)] out TCancelled? value) {
        value = CancelledValue;
        return IsCancelled;
    }

    /// <summary>
    /// Gets the success value if the operation succeeded, otherwise returns the default value for the type.
    /// </summary>
    /// <returns>The success value if the operation succeeded, otherwise default.</returns>
    [Pure]
    public TSuccess? GetSuccessOrDefault() => IsSuccess ? SuccessValue : default;
    /// <summary>
    /// Gets the success value if the operation succeeded, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the operation failed or was cancelled.</param>
    /// <returns>The success value if the operation succeeded, otherwise the specified default value.</returns>
    [Pure]
    public TSuccess GetSuccessOrDefault(TSuccess defaultValue) => IsSuccess ? SuccessValue : defaultValue;
    
    /// <summary>
    /// Returns a string representation of the operation result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded with the success value, failed with the failure value, or was cancelled with the cancellation value.</returns>
    [Pure]
    public override string ToString() => Type switch {
        OperationResultType.Success => $"Success: {SuccessValue}",
        OperationResultType.Failure => $"Failure: {FailureValue}",
        OperationResultType.Cancelled => $"Cancelled: {CancelledValue}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}