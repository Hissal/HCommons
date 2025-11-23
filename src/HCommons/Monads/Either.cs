using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace HCommons.Monads;

/// <summary>
/// Specifies which side of an Either contains a value.
/// </summary>
public enum EitherType : byte {
    /// <summary>
    /// The left side contains a value.
    /// </summary>
    Left,
    /// <summary>
    /// The right side contains a value.
    /// </summary>
    Right
}

/// <summary>
/// Represents a value that is one of two possible types (left or right).
/// </summary>
/// <typeparam name="TLeft">The type of the left value.</typeparam>
/// <typeparam name="TRight">The type of the right value.</typeparam>
public readonly record struct Either<TLeft, TRight>(EitherType Type, TLeft? Left, TRight? Right)
    where TLeft : notnull where TRight : notnull 
{
    /// <summary>
    /// Gets a value indicating whether this either contains a left value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Left))]
    [MemberNotNullWhen(false, nameof(Right))]
    public bool IsLeft => Type == EitherType.Left;

    /// <summary>
    /// Gets a value indicating whether this either contains a right value.
    /// </summary>
    [MemberNotNullWhen(true, nameof(Right))]
    [MemberNotNullWhen(false, nameof(Left))]
    public bool IsRight => Type == EitherType.Right;

    /// <summary>
    /// Creates an either containing a left value.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <returns>An either containing the left value.</returns>
    [Pure]
    public static Either<TLeft, TRight> FromLeft(TLeft left) => new(EitherType.Left, left, default);

    /// <summary>
    /// Creates an either containing a right value.
    /// </summary>
    /// <param name="right">The right value.</param>
    /// <returns>An either containing the right value.</returns>
    [Pure]
    public static Either<TLeft, TRight> FromRight(TRight right) => new(EitherType.Right, default, right);

    /// <summary>
    /// Implicitly converts a left value to an either.
    /// </summary>
    public static implicit operator Either<TLeft, TRight>(TLeft left) => FromLeft(left);
    /// <summary>
    /// Implicitly converts a right value to an either.
    /// </summary>
    public static implicit operator Either<TLeft, TRight>(TRight right) => FromRight(right);

    /// <summary>
    /// Attempts to get the left value.
    /// </summary>
    /// <param name="left">When this method returns, contains the left value if present; otherwise, the default value.</param>
    /// <returns>True if this either contains a left value; otherwise, false.</returns>
    [Pure]
    public bool TryGetLeft([NotNullWhen(true)] out TLeft? left) {
        left = Left;
        return IsLeft;
    }

    /// <summary>
    /// Attempts to get the right value.
    /// </summary>
    /// <param name="right">When this method returns, contains the right value if present; otherwise, the default value.</param>
    /// <returns>True if this either contains a right value; otherwise, false.</returns>
    [Pure]
    public bool TryGetRight([NotNullWhen(true)] out TRight? right) {
        right = Right;
        return IsRight;
    }

    /// <summary>
    /// Gets the left value if present, otherwise returns the default value for the type.
    /// </summary>
    /// <returns>The left value if present, otherwise default.</returns>
    [Pure]
    public TLeft? GetLeftOrDefault() => IsLeft ? Left : default;

    /// <summary>
    /// Gets the left value if present, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if no left value is present.</param>
    /// <returns>The left value if present, otherwise the specified default value.</returns>
    [Pure]
    public TLeft GetLeftOrDefault(TLeft defaultValue) => IsLeft ? Left : defaultValue;

    /// <summary>
    /// Gets the right value if present, otherwise returns the default value for the type.
    /// </summary>
    /// <returns>The right value if present, otherwise default.</returns>
    [Pure]
    public TRight? GetRightOrDefault() => IsRight ? Right : default;

    /// <summary>
    /// Gets the right value if present, otherwise returns the specified default value.
    /// </summary>
    /// <param name="defaultValue">The default value to return if no right value is present.</param>
    /// <returns>The right value if present, otherwise the specified default value.</returns>
    [Pure]
    public TRight GetRightOrDefault(TRight defaultValue) => IsRight ? Right : defaultValue;

    /// <summary>
    /// Returns a string representation of the either.
    /// </summary>
    /// <returns>A string indicating which side contains a value and what that value is.</returns>
    [Pure]
    public override string ToString() => IsLeft ? $"Left: {Left}" : $"Right: {Right}";
}