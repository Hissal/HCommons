using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Represents the result of an operation that can either succeed with a value or fail with an error.
/// </summary>
/// <typeparam name="TValue">The type of the success value.</typeparam>
public readonly record struct Result<TValue> where TValue : notnull {
    /// <summary>
    /// Gets a value indicating whether the operation succeeded.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; init; }
    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailure => !IsSuccess;
    
    /// <summary>
    /// Gets the value if the operation succeeded, otherwise null.
    /// </summary>
    public TValue? Value { get; init; }
    /// <summary>
    /// Gets the error if the operation failed.
    /// </summary>
    public Error Error { get; init; }
        
    Result(bool isSuccess, TValue? value, Error error) {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
        
    /// <summary>
    /// Creates a successful result with the specified value.
    /// </summary>
    /// <param name="value">The success value.</param>
    /// <returns>A successful result.</returns>
    [Pure]
    public static Result<TValue> Success(TValue value) => new (true, value, Error.Empty);
    /// <summary>
    /// Creates a failed result with the specified error.
    /// </summary>
    /// <param name="error">The error that caused the failure.</param>
    /// <returns>A failed result.</returns>
    [Pure]
    public static Result<TValue> Failure(Error error) => new (false, default, error);
        
    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    public static implicit operator Result<TValue>(TValue value) => Success(value);
    /// <summary>
    /// Implicitly converts an error to a failed result.
    /// </summary>
    public static implicit operator Result<TValue>(Error error) => Failure(error);
    
    /// <summary>
    /// Attempts to get the value if the operation succeeded.
    /// </summary>
    /// <param name="value">When this method returns, contains the value if the operation succeeded; otherwise, the default value.</param>
    /// <returns>True if the operation succeeded; otherwise, false.</returns>
    [Pure]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value) {
        value = Value;
        return IsSuccess;
    }
    
    /// <summary>
    /// Attempts to get the error if the operation failed.
    /// </summary>
    /// <param name="error">When this method returns, contains the error if the operation failed; otherwise, the default error.</param>
    /// <returns>True if the operation failed; otherwise, false.</returns>
    [Pure]
    public bool TryGetError(out Error error) {
        error = Error;
        return !IsSuccess;
    }
    
    /// <summary>
    /// Gets the value if the operation succeeded, otherwise returns the default value for the type.
    /// </summary>
    /// <returns>The value if the operation succeeded, otherwise default.</returns>
    [Pure]
    public TValue? GetValueOrDefault() => IsSuccess ? Value : default;
    /// <summary>
    /// Gets the value if the operation succeeded, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the operation failed.</param>
    /// <returns>The value if the operation succeeded, otherwise the specified default value.</returns>
    [Pure]
    public TValue GetValueOrDefault(TValue defaultValue) => IsSuccess ? Value! : defaultValue;
    
    /// <summary>
    /// Matches on the result and returns a value.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="onSuccess">The function to execute if the operation succeeded.</param>
    /// <param name="onFailure">The function to execute if the operation failed.</param>
    /// <returns>The result of the executed function.</returns>
    [Pure]
    public TResult Match<TResult>(
        Func<TValue, TResult> onSuccess, 
        Func<Error, TResult> onFailure) 
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error);
    }
    
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
    public TResult Match<TState, TResult>(
        TState state,
        Func<TState, TValue, TResult> onSuccess, 
        Func<TState, Error, TResult> onFailure) 
    {
        return IsSuccess ? onSuccess(state, Value!) : onFailure(state, Error);
    }
    
    /// <summary>
    /// Executes an action based on whether the operation succeeded or failed.
    /// </summary>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public void Switch(
        Action<TValue> onSuccess, 
        Action<Error> onFailure) 
    {
        if (IsSuccess) onSuccess(Value!);
        else onFailure(Error);
    }
    
    /// <summary>
    /// Executes an action with state based on whether the operation succeeded or failed.
    /// </summary>
    /// <typeparam name="TState">The type of the state.</typeparam>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="onSuccess">The action to execute if the operation succeeded.</param>
    /// <param name="onFailure">The action to execute if the operation failed.</param>
    public void Switch<TState>(
        TState state,
        Action<TState, TValue> onSuccess, 
        Action<TState, Error> onFailure) 
    {
        if (IsSuccess) onSuccess(state, Value!);
        else onFailure(state, Error);
    }
    
    /// <summary>
    /// Returns a string representation of the result.
    /// </summary>
    /// <returns>A string indicating whether the operation succeeded with the value or failed with the error.</returns>
    [Pure]
    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
}