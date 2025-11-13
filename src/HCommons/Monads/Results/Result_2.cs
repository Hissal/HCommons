using System.Diagnostics.CodeAnalysis;

namespace HCommons.Monads;

public readonly record struct Result<TSuccess, TFailure> where TSuccess : notnull where TFailure : notnull {
    [MemberNotNullWhen(true, nameof(Value))]
    [MemberNotNullWhen(false, nameof(FailureValue))]
    public bool IsSuccess { get; }
    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true, nameof(FailureValue))]
    public bool IsFailure => !IsSuccess;
    
    public TSuccess? Value { get; }
    public TFailure? FailureValue { get; }
        
    Result(bool isSuccess, TSuccess? value = default, TFailure? failureValue = default) {
        IsSuccess = isSuccess;
        Value = value;
        FailureValue = failureValue;
    }
        
    public static Result<TSuccess, TFailure> Success(TSuccess value) => new (true, value);
    public static Result<TSuccess, TFailure> Failure(TFailure value) => new (false, default, value);
        
    public static implicit operator Result<TSuccess, TFailure>(TSuccess value) => Success(value);
    public static implicit operator Result<TSuccess, TFailure>(TFailure value) => Failure(value);
    
    public bool TryGetSuccess([NotNullWhen(true)] out TSuccess? value) {
        value = Value;
        return IsSuccess;
    }
    
    public bool TryGetFailure([NotNullWhen(true)] out TFailure? value) {
        value = FailureValue;
        return IsFailure;
    }
    
    public TSuccess? GetSuccessOrDefault() => IsSuccess ? Value : default;
    public TSuccess GetSuccessOrDefault(TSuccess defaultValue) => IsSuccess ? Value : defaultValue;

    public TMatch Match<TMatch>(
        Func<TSuccess, TMatch> onSuccess, 
        Func<TFailure, TMatch> onFailure) 
    {
        return IsSuccess ? onSuccess(Value!) : onFailure(FailureValue!);
    }
    
    public TMatch Match<TState, TMatch>(
        TState state,
        Func<TState, TSuccess, TMatch> onSuccess, 
        Func<TState, TFailure, TMatch> onFailure) 
    {
        return IsSuccess ? onSuccess(state, Value!) : onFailure(state, FailureValue!);
    }
    
    public void Switch(
        Action<TSuccess> onSuccess, 
        Action<TFailure> onFailure) 
    {
        if (IsSuccess) onSuccess(Value!);
        else onFailure(FailureValue!);
    }
    
    public void Switch<TState>(
        TState state,
        Action<TState, TSuccess> onSuccess, 
        Action<TState, TFailure> onFailure) 
    {
        if (IsSuccess) onSuccess(state, Value!);
        else onFailure(state, FailureValue!);
    }
    
    public override string ToString() => IsSuccess ? $"Success: {Value}" : $"Failure: {FailureValue}";
}