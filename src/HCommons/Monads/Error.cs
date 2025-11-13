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
    public static implicit operator string(Error error) => error.Message;

    public override string ToString() => $"[Error]: {Message} {(HasException ? $"| -> [Exception]: {Exception}" : string.Empty)}";
}

public readonly record struct Error<T>(T Value, string Message, Exception? Exception = null) {
    [MemberNotNullWhen(true, nameof(Exception))]
    public bool HasException => Exception is not null;
    [Pure]
    public static Error<T> FromException(T value, Exception ex) => new Error<T>(value, ex.Message, ex);
    [Pure]
    public static Error<T> WithMessage(T value, string message) => new Error<T>(value, message);
    [Pure]
    public static Error<T> ValueOnly(T value) => new Error<T>(value, string.Empty);
    
    public static implicit operator Error(Error<T> error) => new Error(error.Message, error.Exception);
    
    public override string ToString() => $"[Error]: {Message} {(HasException ? $"| -> [Exception]: {Exception}" : string.Empty)} | -> [Value]: {Value}";
}

public static class ErrorExtensions {
    [Pure]
    public static Error<T> WithValue<T>(this Error error, T value) => new Error<T>(value, error.Message, error.Exception);
    [Pure]
    public static Error WithoutValue<T>(this Error<T> error) => new Error(error.Message, error.Exception);
    
    [Pure]
    public static Error WithMessage(this Error error, string message) => new Error(message, error.Exception);
    [Pure]
    public static Error<T> WithMessage<T>(this Error<T> error, string message) => new Error<T>(error.Value, message, error.Exception);
    
    [Pure]
    public static Error WithException(this Error error, Exception exception) => new Error(error.Message, exception);
    [Pure]
    public static Error<T> WithException<T>(this Error<T> error, Exception exception) => new Error<T>(error.Value, error.Message, exception);
}
