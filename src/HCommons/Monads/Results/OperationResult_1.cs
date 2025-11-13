using System.Diagnostics.CodeAnalysis;

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

    public static OperationResult<TValue> Success(TValue value) => new OperationResult<TValue>(OperationResultType.Success, value, Error.Empty, Monads.Cancelled.Empty);
    public static OperationResult<TValue> Failure(Error error) => new OperationResult<TValue>(OperationResultType.Failure, default, error, Monads.Cancelled.Empty);
    public static OperationResult<TValue> Cancelled(Cancelled cancelled) => new OperationResult<TValue>(OperationResultType.Cancelled, default, Error.Empty, cancelled);

    public static implicit operator OperationResult<TValue>(Error error) => Failure(error);
    public static implicit operator OperationResult<TValue>(Cancelled cancelled) => Cancelled(cancelled);
    
    public bool TryGetValue([NotNullWhen(true)] out TValue? value) {
        value = Value;
        return IsSuccess;
    }
    
    public TValue? GetValueOrDefault() => IsSuccess ? Value : default;
    public TValue GetValueOrDefault(TValue defaultValue) => IsSuccess ? Value : defaultValue;
    
    public TMatch Match<TMatch>(
        Func<TValue, TMatch> onSuccess, 
        Func<Error, TMatch> onFailure,
        Func<Cancelled, TMatch> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(Value!),
            OperationResultType.Failure => onFailure(Error),
            OperationResultType.Cancelled => onCancelled(Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
        };
    }
    
    public TMatch Match<TState, TMatch>(
        TState state,
        Func<TState, TValue, TMatch> onSuccess,
        Func<TState, Error, TMatch> onFailure,
        Func<TState, Cancelled, TMatch> onCancelled) 
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

    public override string ToString() => Type switch {
        OperationResultType.Success => $"Success: {Value}",
        OperationResultType.Failure => $"Failure: {Error}",
        OperationResultType.Cancelled => $"Cancelled: {Cancellation}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}