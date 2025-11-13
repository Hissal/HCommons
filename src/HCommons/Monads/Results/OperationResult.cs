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

    public static OperationResult Success() => new OperationResult(OperationResultType.Success, Error.Empty, Monads.Cancelled.Empty);

    public static OperationResult Failure(Error error) => new OperationResult(OperationResultType.Failure, error, Monads.Cancelled.Empty);

    public static OperationResult Cancelled(Cancelled cancelled) => new OperationResult(OperationResultType.Cancelled, Error.Empty, cancelled);

    public static implicit operator OperationResult(Error error) => Failure(error);
    public static implicit operator OperationResult(Cancelled cancelled) => Cancelled(cancelled);

    public TMatch Match<TMatch>(
        Func<TMatch> onSuccess, 
        Func<Error, TMatch> onFailure,
        Func<Cancelled, TMatch> onCancelled) 
    {
        return Type switch {
            OperationResultType.Success => onSuccess(),
            OperationResultType.Failure => onFailure(Error),
            OperationResultType.Cancelled => onCancelled(Cancellation),
            _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
        };
    }

    public TMatch Match<TState, TMatch>(
        TState state,
        Func<TState, TMatch> onSuccess,
        Func<TState, Error, TMatch> onFailure,
        Func<TState, Cancelled, TMatch> onCancelled) 
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

    public override string ToString() => Type switch {
        OperationResultType.Success => "Success",
        OperationResultType.Failure => $"Failure: {Error}",
        OperationResultType.Cancelled => $"Cancelled: {Cancellation}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}