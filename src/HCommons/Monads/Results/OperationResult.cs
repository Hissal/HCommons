using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public enum OperationResultType : byte {
    Success,
    Failure,
    Cancelled
}

public readonly record struct OperationResult(OperationResultType Type, Error Error, Cancelled Cancellation) {
    public bool IsSuccess => Type == OperationResultType.Success;
    public bool IsFailure => Type == OperationResultType.Failure;
    public bool IsCancelled => Type == OperationResultType.Cancelled;

    [Pure]
    public static OperationResult Success() => new OperationResult(OperationResultType.Success, Error.Empty, Monads.Cancelled.Empty);

    [Pure]
    public static OperationResult Failure(Error error) => new OperationResult(OperationResultType.Failure, error, Monads.Cancelled.Empty);

    [Pure]
    public static OperationResult Cancelled(Cancelled cancelled) => new OperationResult(OperationResultType.Cancelled, Error.Empty, cancelled);

    public static implicit operator OperationResult(Error error) => Failure(error);
    public static implicit operator OperationResult(Cancelled cancelled) => Cancelled(cancelled);

    [Pure]
    public TResult Match<TResult>(
        Func<TResult> onSuccess, 
        Func<Error, TResult> onFailure,
        Func<Cancelled, TResult> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(),
            OperationResultType.Failure => onFailure(Error),
            OperationResultType.Cancelled => onCancelled(Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
        };
    }

    [Pure]
    public TResult Match<TState, TResult>(
        TState state,
        Func<TState, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure,
        Func<TState, Cancelled, TResult> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(state),
            OperationResultType.Failure => onFailure(state, Error),
            OperationResultType.Cancelled => onCancelled(state, Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
        };
    }

    public void Switch(Action onSuccess, Action<Error> onFailure, Action<Cancelled> onCancelled) {
        if (IsSuccess) onSuccess();
        else if (IsFailure) onFailure(Error);
        else onCancelled(Cancellation);
    }

    public void Switch<TState>(
        TState state, 
        Action<TState> onSuccess, 
        Action<TState, Error> onFailure,
        Action<TState, Cancelled> onCancelled) 
    {
        if (IsSuccess) onSuccess(state);
        else if (IsFailure) onFailure(state, Error);
        else onCancelled(state, Cancellation);
    }

    [Pure]
    public override string ToString() => Type switch {
        OperationResultType.Success => "Success",
        OperationResultType.Failure => $"Failure: {Error}",
        OperationResultType.Cancelled => $"Cancelled: {Cancellation}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}