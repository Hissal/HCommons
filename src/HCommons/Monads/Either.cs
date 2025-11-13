using System.Diagnostics.CodeAnalysis;

namespace HCommons.Monads;

public enum EitherType : byte {
    Left,
    Right
}

public readonly record struct Either<TLeft, TRight>(EitherType Type, TLeft? Left, TRight? Right) where TLeft : notnull where TRight : notnull {
    [MemberNotNullWhen(true, nameof(Left))]
    [MemberNotNullWhen(false, nameof(Right))]
    public bool IsLeft => Type == EitherType.Left;
    [MemberNotNullWhen(true, nameof(Right))]
    [MemberNotNullWhen(false, nameof(Left))]
    public bool IsRight => Type == EitherType.Right;
    
    public static Either<TLeft, TRight> FromLeft(TLeft left) => new(EitherType.Left, left, default);
    public static Either<TLeft, TRight> FromRight(TRight right) => new(EitherType.Right, default, right);
    
    public static implicit operator Either<TLeft, TRight>(TLeft left) => FromLeft(left);
    public static implicit operator Either<TLeft, TRight>(TRight right) => FromRight(right);
    
    public bool TryGetLeft([NotNullWhen(true)] out TLeft? left) {
        left = Left;
        return IsLeft;
    }
    
    public bool TryGetRight([NotNullWhen(true)] out TRight? right) {
        right = Right;
        return IsRight;
    }
    
    public TLeft? GetLeftOrDefault() => IsLeft ? Left : default;
    public TLeft GetLeftOrDefault(TLeft defaultValue) => IsLeft ? Left : defaultValue;
    
    public TRight? GetRightOrDefault() => IsRight ? Right : default;
    public TRight GetRightOrDefault(TRight defaultValue) => IsRight ? Right : defaultValue;
    
    public TResult Match<TResult>(Func<TLeft, TResult> leftFunc, Func<TRight, TResult> rightFunc) =>
        IsLeft ? leftFunc(Left) : rightFunc(Right);
    
    public TResult Match<TState, TResult>(TState state, Func<TState, TLeft, TResult> leftFunc, Func<TState, TRight, TResult> rightFunc) =>
        IsLeft ? leftFunc(state, Left) : rightFunc(state, Right);
    
    public void Switch(Action<TLeft> leftAction, Action<TRight> rightAction) {
        if (IsLeft) leftAction(Left);
        else rightAction(Right);
    }
    
    public void Switch<TState>(TState state, Action<TState, TLeft> leftAction, Action<TState, TRight> rightAction) {
        if (IsLeft) leftAction(state, Left);
        else rightAction(state, Right);
    }
    
    public override string ToString() => IsLeft ? $"Left: {Left}" : $"Right: {Right}";
}