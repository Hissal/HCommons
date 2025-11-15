using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public readonly record struct Cancelled(string Reason) {
    [Pure]
    public static Cancelled Empty => new Cancelled(string.Empty);
    
    [Pure]
    public static Cancelled Because(string reason) => new Cancelled(reason);
    
    public static implicit operator Cancelled(string reason) => new Cancelled(reason);
    [Pure]
    public override string ToString() => $"[Cancelled]: {Reason}";
}

public readonly record struct Cancelled<TValue>(TValue Value, string Reason) {
    [Pure]
    public static Cancelled<TValue> ValueOnly(TValue value) => new Cancelled<TValue>(value, string.Empty); 
    [Pure]
    public static Cancelled<TValue> Because(TValue value, string reason) => new Cancelled<TValue>(value, reason);
    
    public static implicit operator Cancelled(Cancelled<TValue> cancelled) => new Cancelled(cancelled.Reason);
    [Pure]
    public override string ToString() => $"[Cancelled]: {Reason}, [Value]: {Value}";
}

public static class CancelledExtensions {
    [Pure]
    public static Cancelled<TValue> WithValue<TValue>(this Cancelled cancelled, TValue value) => new Cancelled<TValue>(value, cancelled.Reason);
    [Pure]
    public static Cancelled WithoutValue<TValue>(this Cancelled<TValue> cancelled) => new Cancelled(cancelled.Reason);
}