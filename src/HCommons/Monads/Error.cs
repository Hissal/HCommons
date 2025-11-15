using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public readonly record struct Error(string Message, Exception? Exception = null) {
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool HasException => Exception is not null;
    
    public static Error Empty => new Error(string.Empty);
    
    [Pure]
    public static Error FromException(Exception ex) => new Error(ex.Message, ex);
    [Pure]
    public static Error WithMessage(string message) => new Error(message);
    
    public static implicit operator Error(Exception exception) => FromException(exception);
    public static implicit operator Error(string message) => WithMessage(message);

    public override string ToString() => $"[Error]: {Message}{(HasException ? $" | -> [Exception]: {Exception}" : string.Empty)}";
}

public readonly record struct Error<TValue>(TValue Value, string Message, Exception? Exception = null) {
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool HasException => Exception is not null;
    
    [Pure]
    public static Error<TValue> FromException(TValue value, Exception ex) => new Error<TValue>(value, ex.Message, ex);
    [Pure]
    public static Error<TValue> WithMessage(TValue value, string message) => new Error<TValue>(value, message);
    [Pure]
    public static Error<TValue> ValueOnly(TValue value) => new Error<TValue>(value, string.Empty);
    
    public static implicit operator Error(Error<TValue> error) => new Error(error.Message, error.Exception);
    
    public override string ToString() => $"[Error]: {Message}{(HasException ? $" | -> [Exception]: {Exception}" : string.Empty)} | -> [Value]: {Value}";
}

public static class ErrorExtensions {
    [Pure]
    public static Error<TValue> WithValue<TValue>(this Error error, TValue value) => new Error<TValue>(value, error.Message, error.Exception);
    [Pure]
    public static Error WithoutValue<TValue>(this Error<TValue> error) => new Error(error.Message, error.Exception);
    
    [Pure]
    public static Error WithMessage(this Error error, string message) => new Error(message, error.Exception);
    [Pure]
    public static Error<TValue> WithMessage<TValue>(this Error<TValue> error, string message) => new Error<TValue>(error.Value, message, error.Exception);
    
    [Pure]
    public static Error WithException(this Error error, Exception exception) => new Error(error.Message, exception);
    [Pure]
    public static Error<TValue> WithException<TValue>(this Error<TValue> error, Exception exception) => new Error<TValue>(error.Value, error.Message, exception);
}
