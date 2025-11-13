using System.Diagnostics.CodeAnalysis;

namespace HCommons.Monads;

public readonly record struct Result(bool IsSuccess, Error Error) {
    public bool IsFailure => !IsSuccess;

    public static Result Success() => new Result(true, Error.Empty);
    public static Result Failure(Error error) => new Result(false, error);

    public static implicit operator Result(Error error) => Failure(error);

    public TMatch Match<TMatch>(Func<TMatch> onSuccess, Func<Error, TMatch> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    public TMatch Match<TState, TMatch>(TState state, Func<TState, TMatch> onSuccess,
        Func<TState, Error, TMatch> onFailure) =>
        IsSuccess ? onSuccess(state) : onFailure(state, Error);

    public void Switch(Action onSuccess, Action<Error> onFailure) {
        if (IsSuccess) onSuccess();
        else onFailure(Error);
    }

    public void Switch<TState>(TState state, Action<TState> onSuccess, Action<TState, Error> onFailure) {
        if (IsSuccess) onSuccess(state);
        else onFailure(state, Error);
    }

    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}