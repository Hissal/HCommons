namespace HCommons.Monads;

public static class EitherExtensions {
    public static Either<TRight, TLeft> Swap<TLeft, TRight>(this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft
            ? Either<TRight, TLeft>.FromRight(either.Left)
            : Either<TRight, TLeft>.FromLeft(either.Right);
    }
    
    public static Optional<TLeft> AsLeftOptional<TLeft, TRight>(
        this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft 
            ? Optional<TLeft>.Some(either.Left) 
            : Optional<TLeft>.None();
    }
    
    public static Optional<TRight> AsRightOptional<TLeft, TRight>(
        this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsRight 
            ? Optional<TRight>.Some(either.Right) 
            : Optional<TRight>.None();
    }
    
    // ===== Transformations ===== //
    
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
    
    // ===== Stateful Overloads ===== //
    
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
    
    public static void Switch<TLeft, TRight>(
        this Either<TLeft, TRight> either,
        Action<TLeft> leftAction,
        Action<TRight> rightAction) 
        where TLeft : notnull where TRight : notnull 
    {
        if (either.IsLeft) leftAction(either.Left);
        else rightAction(either.Right);
    }
    
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