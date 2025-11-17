using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Specifies the type of result for an operation.
/// </summary>
public enum OperationResultType : byte {
    /// <summary>
    /// The operation succeeded.
    /// </summary>
    Success,
    /// <summary>
    /// The operation failed.
    /// </summary>
    Failure,
    /// <summary>
    /// The operation was cancelled.
    /// </summary>
    Cancelled
}

/// <summary>
/// Represents the result of an operation that can succeed, fail with an error, or be cancelled.
/// </summary>
public readonly record struct OperationResult(OperationResultType Type, Error Error, Cancelled Cancellation) {
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    public bool IsSuccess => Type == OperationResultType.Success;
    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => Type == OperationResultType.Failure;
    /// <summary>
    /// Gets a value indicating whether the operation was cancelled.
    /// </summary>
    public bool IsCancelled => Type == OperationResultType.Cancelled;

    /// <summary>
    /// Creates a successful operation result.
    /// </summary>
    /// <returns>A successful operation result.</returns>
    [Pure]
    public static OperationResult Success() => new OperationResult(OperationResultType.Success, Error.Empty, Monads.Cancelled.Empty);

    /// <summary>
    /// Creates a failed operation result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed operation result.</returns>
    [Pure]
    public static OperationResult Failure(Error error) => new OperationResult(OperationResultType.Failure, error, Monads.Cancelled.Empty);

    /// <summary>
    /// Creates a cancelled operation result with the specified cancellation.
    /// </summary>
    /// <param name="cancelled">The cancellation information.</param>
    /// <returns>A cancelled operation result.</returns>
    [Pure]
    public static OperationResult Cancelled(Cancelled cancelled) => new OperationResult(OperationResultType.Cancelled, Error.Empty, cancelled);

    /// <summary>
    /// Implicitly converts an error to a failed operation result.
    /// </summary>
    public static implicit operator OperationResult(Error error) => Failure(error);
    /// <summary>
    /// Implicitly converts a cancellation to a cancelled operation result.
    /// </summary>
    public static implicit operator OperationResult(Cancelled cancelled) => Cancelled(cancelled);

    /// <summary>
    /// Matches on the operation result and returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
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

    /// <summary>
    /// Matches on the operation result with state and returns a value.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <param name="onCancelled">The function to execute if the operation was cancelled.</param>
    /// <returns>The result of the executed function.</returns>
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

    /// <summary>
    /// Executes an action based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
    public void Switch(Action onSuccess, Action<Error> onFailure, Action<Cancelled> onCancelled) {
        if (IsSuccess) onSuccess();
        else if (IsFailure) onFailure(Error);
        else onCancelled(Cancellation);
    }

    /// <summary>
    /// Executes an action with state based on whether the operation succeeded, failed, or was cancelled.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    /// <param name="onCancelled">The action to execute if the operation was cancelled.</param>
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

    /// <summary>
    /// Returns a string representation of the operation result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded, failed with the error, or was cancelled with the cancellation information.</returns>
    [Pure]
    public override string ToString() => Type switch {
        OperationResultType.Success => "Success",
        OperationResultType.Failure => $"Failure: {Error}",
        OperationResultType.Cancelled => $"Cancelled: {Cancellation}",
        _ => throw new InvalidOperationException($"Unknown OperationResultType: {Type}")
    };
}