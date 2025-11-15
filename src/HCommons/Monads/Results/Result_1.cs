using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public readonly record struct Result<TValue> where TValue : notnull {
    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess { get; init; }
    [MemberNotNullWhen(false, nameof(Value))]
    public bool IsFailure => !IsSuccess;
    
    public TValue? Value { get; init; }
    public Error Error { get; init; }
        
    Result(bool isSuccess, TValue? value, Error error) {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }
        
    [Pure]
    public static Result<TValue> Success(TValue value) => new (true, value, Error.Empty);
    [Pure]
    public static Result<TValue> Failure(Error error) => new (false, default, error);
        
    public static implicit operator Result<TValue>(TValue value) => Success(value);
    public static implicit operator Result<TValue>(Error error) => Failure(error);
    
    [Pure]
    public bool TryGetValue([NotNullWhen(true)] out TValue? value) {
        value = Value;
        return IsSuccess;
    }
    
    [Pure]
    public bool TryGetError(out Error error) {
        error = Error;
        return !IsSuccess;
    }
    
    [Pure]
    public TValue? GetValueOrDefault() => IsSuccess ? Value : default;
    [Pure]
    public TValue GetValueOrDefault(TValue defaultValue) => IsSuccess ? Value! : defaultValue;
    
    [Pure]
    public TMatch Match<TMatch>(
        Func<TValue, TMatch> onSuccess, 
        Func<Error, TMatch> onFailure) 
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(Error);
    }
    
    [Pure]
    public TMatch Match<TState, TMatch>(
        TState state,
        Func<TState, TValue, TMatch> onSuccess, 
        Func<TState, Error, TMatch> onFailure) 
    {
        return IsSuccess ? onSuccess(state, Value!) : onFailure(state, Error);
    }
    
    public void Switch(
        Action<TValue> onSuccess, 
        Action<Error> onFailure) 
    {
        if (IsSuccess) onSuccess(Value!);
        else onFailure(Error);
    }
    
    public void Switch<TState>(
        TState state,
        Action<TState, TValue> onSuccess, 
        Action<TState, Error> onFailure) 
    {
        if (IsSuccess) onSuccess(state, Value!);
        else onFailure(state, Error);
    }
    
    [Pure]
    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {Error}";
}