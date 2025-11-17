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
    /// Matches on the result and returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public TResult Match<TResult>(Func<TResult> onSuccess, Func<Error, TResult> onFailure) =>
        IsSuccess ? onSuccess() : onFailure(Error);

    /// <summary>
    /// Matches on the result with state and returns a value.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public TResult Match<TState, TResult>(TState state, Func<TState, TResult> onSuccess,
        Func<TState, Error, TResult> onFailure) =>
        IsSuccess ? onSuccess(state) : onFailure(state, Error);

    /// <summary>
    /// Executes an action based on whether the operation succeeded or failed.
    /// </summary>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public void Switch(Action onSuccess, Action<Error> onFailure) {
        if (IsSuccess) onSuccess();
        else onFailure(Error);
    }

    /// <summary>
    /// Executes an action with state based on whether the operation succeeded or failed.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public void Switch<TState>(TState state, Action<TState> onSuccess, Action<TState, Error> onFailure) {
        if (IsSuccess) onSuccess(state);
        else onFailure(state, Error);
    }

    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded or failed with the error.</returns>
    [Pure]
    public override string ToString() => IsSuccess ? "Success" : $"Failure: {Error}";
}