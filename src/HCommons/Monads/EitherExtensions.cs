using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public static class EitherExtensions {
    /// <summary>
    /// Swaps the left and right sides of an either.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="either">The either to swap.</param>
    /// <returns>An either with the left and right sides swapped.</returns>
    [Pure]
    public static Either<TRight, TLeft> Swap<TLeft, TRight>(this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft
            ? Either<TRight, TLeft>.FromRight(either.Left)
            : Either<TRight, TLeft>.FromLeft(either.Right);
    }
    
    /// <summary>
    /// Converts the left side of an either to an optional.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="either">The either to convert.</param>
    /// <returns>An optional containing the left value if present, otherwise none.</returns>
    [Pure]
    public static Optional<TLeft> AsLeftOptional<TLeft, TRight>(
        this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft 
            ? Optional<TLeft>.Some(either.Left) 
            : Optional<TLeft>.None();
    }
    
    /// <summary>
    /// Converts the right side of an either to an optional.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="either">The either to convert.</param>
    /// <returns>An optional containing the right value if present, otherwise none.</returns>
    [Pure]
    public static Optional<TRight> AsRightOptional<TLeft, TRight>(
        this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsRight 
            ? Optional<TRight>.Some(either.Right) 
            : Optional<TRight>.None();
    }
    
    // ===== Transformations ===== //
    
    /// <summary>
    /// Maps both sides of an either using the respective mapper functions.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="leftMapper">The function to apply to the left value.</param>
    /// <param name="rightMapper">The function to apply to the right value.</param>
    /// <returns>An either with the transformed value.</returns>
    [Pure]
    public static Either<TLeftResult, TRightResult> Map<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, TLeftResult> leftMapper,
        Func<TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRightResult>.FromLeft(leftMapper(either.Left))
            : Either<TLeftResult, TRightResult>.FromRight(rightMapper(either.Right));
    }
    
    /// <summary>
    /// Maps the left side of an either using the provided mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="leftMapper">The function to apply to the left value.</param>
    /// <returns>An either with the transformed left value, or the original right value.</returns>
    [Pure]
    public static Either<TLeftResult, TRight> MapLeft<TLeft, TRight, TLeftResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, TLeftResult> leftMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRight>.FromLeft(leftMapper(either.Left))
            : Either<TLeftResult, TRight>.FromRight(either.Right);
    }
    
    /// <summary>
    /// Maps the right side of an either using the provided mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="rightMapper">The function to apply to the right value.</param>
    /// <returns>An either with the transformed right value, or the original left value.</returns>
    [Pure]
    public static Either<TLeft, TRightResult> MapRight<TLeft, TRight, TRightResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeft, TRightResult>.FromLeft(either.Left)
            : Either<TLeft, TRightResult>.FromRight(rightMapper(either.Right));
    }
    
    /// <summary>
    /// Matches an either against two functions and returns the result of the matching function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="either">The either to match.</param>
    /// <param name="leftFunc">The function to apply if the either contains a left value.</param>
    /// <param name="rightFunc">The function to apply if the either contains a right value.</param>
    /// <returns>The result of the matching function.</returns>
    [Pure]
    public static TResult Match<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, TResult> leftFunc,
        Func<TRight, TResult> rightFunc) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft 
            ? leftFunc(either.Left) 
            : rightFunc(either.Right);
    }
    
    // ===== Stateful Transformation Overloads ===== //
    
    /// <summary>
    /// Maps both sides of an either using the respective mapper functions with additional state.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <typeparam name="TState">The type of the state to pass to the mapper functions.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="state">The state to pass to the mapper functions.</param>
    /// <param name="leftMapper">The function to apply to the left value with state.</param>
    /// <param name="rightMapper">The function to apply to the right value with state.</param>
    /// <returns>An either with the transformed value.</returns>
    [Pure]
    public static Either<TLeftResult, TRightResult> Map<TLeft, TRight, TLeftResult, TRightResult, TState>(
        this Either<TLeft, TRight> either,
        TState state,
        Func<TState, TLeft, TLeftResult> leftMapper,
        Func<TState, TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRightResult>.FromLeft(leftMapper(state, either.Left))
            : Either<TLeftResult, TRightResult>.FromRight(rightMapper(state, either.Right));
    }
    
    /// <summary>
    /// Maps the left side of an either using the provided mapper function with additional state.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TState">The type of the state to pass to the mapper function.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="state">The state to pass to the mapper function.</param>
    /// <param name="leftMapper">The function to apply to the left value with state.</param>
    /// <returns>An either with the transformed left value, or the original right value.</returns>
    [Pure]
    public static Either<TLeftResult, TRight> MapLeft<TLeft, TRight, TLeftResult, TState>(
        this Either<TLeft, TRight> either,
        TState state,
        Func<TState, TLeft, TLeftResult> leftMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRight>.FromLeft(leftMapper(state, either.Left))
            : Either<TLeftResult, TRight>.FromRight(either.Right);
    }
    
    /// <summary>
    /// Maps the right side of an either using the provided mapper function with additional state.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <typeparam name="TState">The type of the state to pass to the mapper function.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="state">The state to pass to the mapper function.</param>
    /// <param name="rightMapper">The function to apply to the right value with state.</param>
    /// <returns>An either with the transformed right value, or the original left value.</returns>
    [Pure]
    public static Either<TLeft, TRightResult> MapRight<TLeft, TRight, TRightResult, TState>(
        this Either<TLeft, TRight> either,
        TState state,
        Func<TState, TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeft, TRightResult>.FromLeft(either.Left)
            : Either<TLeft, TRightResult>.FromRight(rightMapper(state, either.Right));
    }
    
    /// <summary>
    /// Matches an either against two functions with additional state and returns the result of the matching function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TState">The type of the state to pass to the functions.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="either">The either to match.</param>
    /// <param name="state">The state to pass to the functions.</param>
    /// <param name="leftFunc">The function to apply if the either contains a left value.</param>
    /// <param name="rightFunc">The function to apply if the either contains a right value.</param>
    /// <returns>The result of the matching function.</returns>
    [Pure]
    public static TResult Match<TLeft, TRight, TState, TResult>(
        this Either<TLeft, TRight> either,
        TState state,
        Func<TState, TLeft, TResult> leftFunc,
        Func<TState, TRight, TResult> rightFunc) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft 
            ? leftFunc(state, either.Left) 
            : rightFunc(state, either.Right);
    }
    
    // ===== Switch ===== //
    
    /// <summary>
    /// Executes one of two actions depending on which side of the either contains a value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="either">The either to switch on.</param>
    /// <param name="leftAction">The action to execute if the either contains a left value.</param>
    /// <param name="rightAction">The action to execute if the either contains a right value.</param>
    public static void Switch<TLeft, TRight>(
        this Either<TLeft, TRight> either,
        Action<TLeft> leftAction,
        Action<TRight> rightAction) 
        where TLeft : notnull where TRight : notnull 
    {
        if (either.IsLeft) leftAction(either.Left);
        else rightAction(either.Right);
    }
    
    /// <summary>
    /// Executes one of two actions with additional state depending on which side of the either contains a value.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TState">The type of the state to pass to the actions.</typeparam>
    /// <param name="either">The either to switch on.</param>
    /// <param name="state">The state to pass to the actions.</param>
    /// <param name="leftAction">The action to execute if the either contains a left value.</param>
    /// <param name="rightAction">The action to execute if the either contains a right value.</param>
    public static void Switch<TLeft, TRight, TState>(
        this Either<TLeft, TRight> either,
        TState state,
        Action<TState, TLeft> leftAction,
        Action<TState, TRight> rightAction) 
        where TLeft : notnull where TRight : notnull 
    {
        if (either.IsLeft) leftAction(state, either.Left);
        else rightAction(state, either.Right);
    }
}