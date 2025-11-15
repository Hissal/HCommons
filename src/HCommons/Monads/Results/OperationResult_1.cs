using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public readonly record struct OperationResult<TValue>(
    OperationResultType Type,
    TValue? Value,
    Error Error,
    Cancelled Cancellation) 
{
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Type == OperationResultType.Success;

    public bool IsFailure => Type == OperationResultType.Failure;
    public bool IsCancelled => Type == OperationResultType.Cancelled;

    [Pure]
    public static OperationResult<TValue> Success(TValue value) => new OperationResult<TValue>(OperationResultType.Success, value, Error.Empty, Monads.Cancelled.Empty);
    [Pure]
    public static OperationResult<TValue> Failure(Error error) => new OperationResult<TValue>(OperationResultType.Failure, default, error, Monads.Cancelled.Empty);
    [Pure]
    public static OperationResult<TValue> Cancelled(Cancelled cancelled) => new OperationResult<TValue>(OperationResultType.Cancelled, default, Error.Empty, cancelled);

    public static implicit operator OperationResult<TValue>(Error error) => Failure(error);
    public static implicit operator OperationResult<TValue>(Cancelled cancelled) => Cancelled(cancelled);
    
    [Pure]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value) {
        value = Value;
        return IsSuccess;
    }
    
    [Pure]
    public TValue? GetValueOrDefault() => IsSuccess ? Value : default;
    [Pure]
    public TValue GetValueOrDefault(TValue defaultValue) => IsSuccess ? Value : defaultValue;
    
    [Pure]
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess, 
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(Value!),
            OperationResultType.Failure => onFailure(Error),
            OperationResultType.Cancelled => onCancelled(Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
        };
    }
    
    [Pure]
    public TResult Match<TState, TResult>(
        TState state,
        Func<TState, TValue, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure,
        Func<TState, Cancelled, TResult> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(state, Value!),
            OperationResultType.Failure => onFailure(state, Error),
            OperationResultType.Cancelled => onCancelled(state, Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
        };
    }
    
    public void Switch(Action<TValue> onSuccess, Action<Error> onFailure, Action<Cancelled> onCancelled) {
        if (IsSuccess) onSuccess(Value);
        else if (IsFailure) onFailure(Error);
        else onCancelled(Cancellation);
    }
    
    public void Switch<TState>(
        TState state, 
        Action<TState, TValue> onSuccess, 
        Action<TState, Error> onFailure,
        Action<TState, Cancelled> onCancelled) 
    {
        if (IsSuccess) onSuccess(state, Value);
        else if (IsFailure) onFailure(state, Error);
        else onCancelled(state, Cancellation);
    }

    [Pure]
    public override string ToString() => Type switch {
        OperationResultType.Success => $"Success: {Value}",
        OperationResultType.Failure => $"Failure: {Error}",
        OperationResultType.Cancelled => $"Cancelled: {Cancellation}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}