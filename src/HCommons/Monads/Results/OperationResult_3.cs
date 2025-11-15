using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

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
    [MemberNotNullWhen(true, nameof(SuccessValue))]
    public bool IsSuccess => Type == OperationResultType.Success;
    [MemberNotNullWhen(true, nameof(FailureValue))]
    public bool IsFailure => Type == OperationResultType.Failure;
    [MemberNotNullWhen(true, nameof(CancelledValue))]
    public bool IsCancelled => Type == OperationResultType.Cancelled;
    
    public static implicit operator OperationResult<TSuccess, TFailure, TCancelled>(TSuccess value) => Success(value);
    public static implicit operator OperationResult<TSuccess, TFailure, TCancelled>(TFailure failure) => Failure(failure);
    public static implicit operator OperationResult<TSuccess, TFailure, TCancelled>(TCancelled cancelled) => Cancelled(cancelled);
    
    [Pure]
    public static OperationResult<TSuccess, TFailure, TCancelled> Success(TSuccess value) => new (OperationResultType.Success, SuccessValue: value);
    [Pure]
    public static OperationResult<TSuccess, TFailure, TCancelled> Failure(TFailure value) => new (OperationResultType.Failure, FailureValue: value);
    [Pure]
    public static OperationResult<TSuccess, TFailure, TCancelled> Cancelled(TCancelled value) => new (OperationResultType.Cancelled, CancelledValue: value);
        
    [Pure]
    public bool TryGetSuccess([NotNullWhen(true)] out TSuccess? value) {
        value = SuccessValue;
        return IsSuccess;
    }
    
    [Pure]
    public bool TryGetFailure([NotNullWhen(true)] out TFailure? value) {
        value = FailureValue;
        return IsFailure;
    }
    
    [Pure]
    public bool TryGetCancelled([NotNullWhen(true)] out TCancelled? value) {
        value = CancelledValue;
        return IsCancelled;
    }

    [Pure]
    public TSuccess? GetSuccessOrDefault() => IsSuccess ? SuccessValue : default;
    [Pure]
    public TSuccess GetSuccessOrDefault(TSuccess defaultValue) => IsSuccess ? SuccessValue : defaultValue;

    [Pure]
    public TMatch Match<TMatch>(
        Func<TSuccess, TMatch> onSuccess, 
        Func<TFailure, TMatch> onFailure, 
        Func<TCancelled, TMatch> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(SuccessValue!),
            OperationResultType.Failure => onFailure(FailureValue!),
            OperationResultType.Cancelled => onCancelled(CancelledValue!),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    [Pure]
    public TMatch Match<TState, TMatch>(
        TState state,
        Func<TState, TSuccess, TMatch> onSuccess, 
        Func<TState, TFailure, TMatch> onFailure, 
        Func<TState, TCancelled, TMatch> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(state, SuccessValue!),
            OperationResultType.Failure => onFailure(state, FailureValue!),
            OperationResultType.Cancelled => onCancelled(state, CancelledValue!),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public void Switch(
        Action<TSuccess> onSuccess, 
        Action<TFailure> onFailure, 
        Action<TCancelled> onCancelled) 
    {
        switch (Type) {
            case OperationResultType.Success:
                onSuccess(SuccessValue!);
                break;
            case OperationResultType.Failure:
                onFailure(FailureValue!);
                break;
            case OperationResultType.Cancelled:
                onCancelled(CancelledValue!);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public void Switch<TState>(
        TState state,
        Action<TState, TSuccess> onSuccess, 
        Action<TState, TFailure> onFailure, 
        Action<TState, TCancelled> onCancelled) 
    {
        switch (Type) {
            case OperationResultType.Success:
                onSuccess(state, SuccessValue!);
                break;
            case OperationResultType.Failure:
                onFailure(state, FailureValue!);
                break;
            case OperationResultType.Cancelled:
                onCancelled(state, CancelledValue!);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    [Pure]
    public override string ToString() => Type switch {
        OperationResultType.Success => $"Success: {SuccessValue}",
        OperationResultType.Failure => $"Failure: {FailureValue}",
        OperationResultType.Cancelled => $"Cancelled: {CancelledValue}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}