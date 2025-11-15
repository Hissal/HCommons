using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public readonly record struct Result(bool IsSuccess, Error Error) {
    public bool IsFailure => !IsSuccess;

    [Pure]
    public static Result Success() => new Result(true, Error.Empty);
    [Pure]
    public static Result Failure(Error error) => new Result(false, error);

    public static implicit operator Result(Error error) => Failure(error);

    [Pure]
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    [Pure]
    public TResult Match<TState, TResult>(TState state, Func<TState, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(state) : onFailure(state, Error);

    public void Switch(Action onSuccess, Action<Error> onFailure) {
        if (IsSuccess) onSuccess();
        else onFailure(Error);
    }

    public void Switch<TState>(TState state, Action<TState> onSuccess, Action<TState, Error> onFailure) {
        if (IsSuccess) onSuccess(state);
        else onFailure(state, Error);
    }

    [Pure]
    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}