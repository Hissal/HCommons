namespace HCommons.Monads;

public readonly record struct Cancelled(string Reason) {
    public static Cancelled Empty => new Cancelled(string.Empty);
    
    public static Cancelled Because(string reason) => new Cancelled(reason);
    
    public static implicit operator Cancelled(string reason) => new Cancelled(reason);
    public static implicit operator string(Cancelled cancelled) => cancelled.Reason;
    
    public override string ToString() => $"[Cancelled]: {Reason}";
}

public readonly record struct Cancelled<T>(T Value, string Reason) {
    public static Cancelled<T> ValueOnly(T value) => new Cancelled<T>(value, string.Empty); 
    public static Cancelled<T> Because(T value, string reason) => new Cancelled<T>(value, reason);
    
    public static implicit operator Cancelled(Cancelled<T> cancelled) => new Cancelled(cancelled.Reason);
    
    public override string ToString() => $"[Cancelled]: {Reason}, [Value]: {Value}";
}

public static class CancelledExtensions {
    public static Cancelled<T> WithValue<T>(this Cancelled cancelled, T value) => new Cancelled<T>(value, cancelled.Reason);
}