using System.Diagnostics.Contracts;

namespace HCommons.Monads;

public static class EitherExtensionsAsync {
    
    /// <summary>
    /// Asynchronously swaps the left and right sides of an either task.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="eitherTask">The either task to swap.</param>
    /// <returns>A task representing the asynchronous operation with an either with the left and right sides swapped.</returns>
    [Pure]
    public static async Task<Either<TRight, TLeft>> SwapAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask) 
        where TLeft : notnull where TRight : notnull 
    {
        return (await eitherTask).Swap();
    }
    
    /// <summary>
    /// Asynchronously swaps the left and right sides of an either.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="either">The either to swap.</param>
    /// <returns>A task representing the asynchronous operation with an either with the left and right sides swapped.</returns>
    [Pure]
    public static async Task<Either<TRight, TLeft>> SwapAsync<TLeft, TRight>(
        this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return await Task.FromResult(either.Swap());
    }
    
    /// <summary>
    /// Asynchronously converts the left side of an either task to an optional.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="eitherTask">The either task to convert.</param>
    /// <returns>A task representing the asynchronous operation with an optional containing the left value if present, otherwise none.</returns>
    [Pure]
    public static async Task<Optional<TLeft>> AsLeftOptionalAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask) 
        where TLeft : notnull where TRight : notnull 
    {
        return (await eitherTask).AsLeftOptional();
    }
    
    /// <summary>
    /// Asynchronously converts the left side of an either to an optional.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="either">The either to convert.</param>
    /// <returns>A task representing the asynchronous operation with an optional containing the left value if present, otherwise none.</returns>
    [Pure]
    public static async Task<Optional<TLeft>> AsLeftOptionalAsync<TLeft, TRight>(
        this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return await Task.FromResult(either.AsLeftOptional());
    }
    
    /// <summary>
    /// Asynchronously converts the right side of an either task to an optional.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="eitherTask">The either task to convert.</param>
    /// <returns>A task representing the asynchronous operation with an optional containing the right value if present, otherwise none.</returns>
    [Pure]
    public static async Task<Optional<TRight>> AsRightOptionalAsync<TLeft, TRight>(
        this Task<Either<TLeft, TRight>> eitherTask) 
        where TLeft : notnull where TRight : notnull 
    {
        return (await eitherTask).AsRightOptional();
    }
    
    /// <summary>
    /// Asynchronously converts the right side of an either to an optional.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <param name="either">The either to convert.</param>
    /// <returns>A task representing the asynchronous operation with an optional containing the right value if present, otherwise none.</returns>
    [Pure]
    public static async Task<Optional<TRight>> AsRightOptionalAsync<TLeft, TRight>(
        this Either<TLeft, TRight> either) 
        where TLeft : notnull where TRight : notnull 
    {
        return await Task.FromResult(either.AsRightOptional());
    }
    
    // ===== Map Async ===== //
    
    /// <summary>
    /// Asynchronously maps both sides of an either task using the respective mapper functions.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="leftMapper">The function to apply to the left value.</param>
    /// <param name="rightMapper">The function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed either.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRightResult>> MapAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TLeftResult> leftMapper,
        Func<TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return (await eitherTask).Map(leftMapper, rightMapper);
    }
    
    /// <summary>
    /// Asynchronously maps both sides of an either using an async left mapper and sync right mapper.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="leftMapperAsync">The async function to apply to the left value.</param>
    /// <param name="rightMapper">The function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed either.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRightResult>> MapAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task<TLeftResult>> leftMapperAsync,
        Func<TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRightResult>.FromLeft(await leftMapperAsync(either.Left))
            : Either<TLeftResult, TRightResult>.FromRight(rightMapper(either.Right));
    }
    
    /// <summary>
    /// Asynchronously maps both sides of an either using a sync left mapper and async right mapper.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="leftMapper">The function to apply to the left value.</param>
    /// <param name="rightMapperAsync">The async function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed either.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRightResult>> MapAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, TLeftResult> leftMapper,
        Func<TRight, Task<TRightResult>> rightMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRightResult>.FromLeft(leftMapper(either.Left))
            : Either<TLeftResult, TRightResult>.FromRight(await rightMapperAsync(either.Right));
    }
    
    /// <summary>
    /// Asynchronously maps both sides of an either task using an async left mapper and sync right mapper.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="leftMapperAsync">The async function to apply to the left value.</param>
    /// <param name="rightMapper">The function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed either.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRightResult>> MapAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TLeftResult>> leftMapperAsync,
        Func<TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return await (await eitherTask).MapAsync(leftMapperAsync, rightMapper);
    }
    
    /// <summary>
    /// Asynchronously maps both sides of an either task using a sync left mapper and async right mapper.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="leftMapper">The function to apply to the left value.</param>
    /// <param name="rightMapperAsync">The async function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed either.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRightResult>> MapAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TLeftResult> leftMapper,
        Func<TRight, Task<TRightResult>> rightMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return await (await eitherTask).MapAsync(leftMapper, rightMapperAsync);
    }
    
    /// <summary>
    /// Asynchronously maps both sides of an either using async mapper functions.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="leftMapperAsync">The async function to apply to the left value.</param>
    /// <param name="rightMapperAsync">The async function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed either.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRightResult>> MapAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task<TLeftResult>> leftMapperAsync,
        Func<TRight, Task<TRightResult>> rightMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRightResult>.FromLeft(await leftMapperAsync(either.Left))
            : Either<TLeftResult, TRightResult>.FromRight(await rightMapperAsync(either.Right));
    }
    
    /// <summary>
    /// Asynchronously maps both sides of an either task using async mapper functions.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="leftMapperAsync">The async function to apply to the left value.</param>
    /// <param name="rightMapperAsync">The async function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed either.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRightResult>> MapAsync<TLeft, TRight, TLeftResult, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TLeftResult>> leftMapperAsync,
        Func<TRight, Task<TRightResult>> rightMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull where TRightResult : notnull 
    {
        return await (await eitherTask).MapAsync(leftMapperAsync, rightMapperAsync);
    }
    
    // ===== Map Left Async ===== //
    
    /// <summary>
    /// Asynchronously maps the left side of an either task using the provided mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="leftMapper">The function to apply to the left value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed left value, or the original right value.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRight>> MapLeftAsync<TLeft, TRight, TLeftResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TLeftResult> leftMapper) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull 
    {
        return (await eitherTask).MapLeft(leftMapper);
    }
    
    /// <summary>
    /// Asynchronously maps the left side of an either using an async mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="leftMapperAsync">The async function to apply to the left value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed left value, or the original right value.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRight>> MapLeftAsync<TLeft, TRight, TLeftResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task<TLeftResult>> leftMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeftResult, TRight>.FromLeft(await leftMapperAsync(either.Left))
            : Either<TLeftResult, TRight>.FromRight(either.Right);
    }
    
    /// <summary>
    /// Asynchronously maps the left side of an either task using an async mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TLeftResult">The result type of the left mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="leftMapperAsync">The async function to apply to the left value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed left value, or the original right value.</returns>
    [Pure]
    public static async Task<Either<TLeftResult, TRight>> MapLeftAsync<TLeft, TRight, TLeftResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TLeftResult>> leftMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TLeftResult : notnull 
    {
        return await (await eitherTask).MapLeftAsync(leftMapperAsync);
    }
    
    // ===== Map Right Async ===== //
    
    /// <summary>
    /// Asynchronously maps the right side of an either task using the provided mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="rightMapper">The function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed right value, or the original left value.</returns>
    [Pure]
    public static async Task<Either<TLeft, TRightResult>> MapRightAsync<TLeft, TRight, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, TRightResult> rightMapper) 
        where TLeft : notnull where TRight : notnull 
        where TRightResult : notnull 
    {
        return (await eitherTask).MapRight(rightMapper);
    }
    
    /// <summary>
    /// Asynchronously maps the right side of an either using an async mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="either">The either to map.</param>
    /// <param name="rightMapperAsync">The async function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed right value, or the original left value.</returns>
    [Pure]
    public static async Task<Either<TLeft, TRightResult>> MapRightAsync<TLeft, TRight, TRightResult>(
        this Either<TLeft, TRight> either,
        Func<TRight, Task<TRightResult>> rightMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TRightResult : notnull 
    {
        return either.IsLeft
            ? Either<TLeft, TRightResult>.FromLeft(either.Left)
            : Either<TLeft, TRightResult>.FromRight(await rightMapperAsync(either.Right));
    }
    
    /// <summary>
    /// Asynchronously maps the right side of an either task using an async mapper function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TRightResult">The result type of the right mapper.</typeparam>
    /// <param name="eitherTask">The either task to map.</param>
    /// <param name="rightMapperAsync">The async function to apply to the right value.</param>
    /// <returns>A task representing the asynchronous operation with the transformed right value, or the original left value.</returns>
    [Pure]
    public static async Task<Either<TLeft, TRightResult>> MapRightAsync<TLeft, TRight, TRightResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TRight, Task<TRightResult>> rightMapperAsync) 
        where TLeft : notnull where TRight : notnull 
        where TRightResult : notnull 
    {
        return await (await eitherTask).MapRightAsync(rightMapperAsync);
    }
    
    // ===== Match Async ===== //
    
    /// <summary>
    /// Asynchronously matches an either task against two functions and returns the result of the matching function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="eitherTask">The either task to match.</param>
    /// <param name="leftFunc">The function to apply if the either contains a left value.</param>
    /// <param name="rightFunc">The function to apply if the either contains a right value.</param>
    /// <returns>A task representing the asynchronous operation with the result of the matching function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TResult> leftFunc,
        Func<TRight, TResult> rightFunc) 
        where TLeft : notnull where TRight : notnull 
    {
        return (await eitherTask).Match(leftFunc, rightFunc);
    }
    
    /// <summary>
    /// Asynchronously matches an either using an async left function and sync right function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="either">The either to match.</param>
    /// <param name="leftFuncAsync">The async function to apply if the either contains a left value.</param>
    /// <param name="rightFunc">The function to apply if the either contains a right value.</param>
    /// <returns>A task representing the asynchronous operation with the result of the matching function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task<TResult>> leftFuncAsync,
        Func<TRight, TResult> rightFunc) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft 
            ? await leftFuncAsync(either.Left) 
            : rightFunc(either.Right);
    }
    
    /// <summary>
    /// Asynchronously matches an either using a sync left function and async right function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="either">The either to match.</param>
    /// <param name="leftFunc">The function to apply if the either contains a left value.</param>
    /// <param name="rightFuncAsync">The async function to apply if the either contains a right value.</param>
    /// <returns>A task representing the asynchronous operation with the result of the matching function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, TResult> leftFunc,
        Func<TRight, Task<TResult>> rightFuncAsync) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft 
            ? leftFunc(either.Left) 
            : await rightFuncAsync(either.Right);
    }
    
    /// <summary>
    /// Asynchronously matches an either task using an async left function and sync right function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="eitherTask">The either task to match.</param>
    /// <param name="leftFuncAsync">The async function to apply if the either contains a left value.</param>
    /// <param name="rightFunc">The function to apply if the either contains a right value.</param>
    /// <returns>A task representing the asynchronous operation with the result of the matching function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TResult>> leftFuncAsync,
        Func<TRight, TResult> rightFunc) 
        where TLeft : notnull where TRight : notnull 
    {
        return await (await eitherTask).MatchAsync(leftFuncAsync, rightFunc);
    }
    
    /// <summary>
    /// Asynchronously matches an either task using a sync left function and async right function.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="eitherTask">The either task to match.</param>
    /// <param name="leftFunc">The function to apply if the either contains a left value.</param>
    /// <param name="rightFuncAsync">The async function to apply if the either contains a right value.</param>
    /// <returns>A task representing the asynchronous operation with the result of the matching function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, TResult> leftFunc,
        Func<TRight, Task<TResult>> rightFuncAsync) 
        where TLeft : notnull where TRight : notnull 
    {
        return await (await eitherTask).MatchAsync(leftFunc, rightFuncAsync);
    }
    
    /// <summary>
    /// Asynchronously matches an either using async functions for both sides.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="either">The either to match.</param>
    /// <param name="leftFuncAsync">The async function to apply if the either contains a left value.</param>
    /// <param name="rightFuncAsync">The async function to apply if the either contains a right value.</param>
    /// <returns>A task representing the asynchronous operation with the result of the matching function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Either<TLeft, TRight> either,
        Func<TLeft, Task<TResult>> leftFuncAsync,
        Func<TRight, Task<TResult>> rightFuncAsync) 
        where TLeft : notnull where TRight : notnull 
    {
        return either.IsLeft 
            ? await leftFuncAsync(either.Left) 
            : await rightFuncAsync(either.Right);
    }
    
    /// <summary>
    /// Asynchronously matches an either task using async functions for both sides.
    /// </summary>
    /// <typeparam name="TLeft">The type of the left value.</typeparam>
    /// <typeparam name="TRight">The type of the right value.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="eitherTask">The either task to match.</param>
    /// <param name="leftFuncAsync">The async function to apply if the either contains a left value.</param>
    /// <param name="rightFuncAsync">The async function to apply if the either contains a right value.</param>
    /// <returns>A task representing the asynchronous operation with the result of the matching function.</returns>
    [Pure]
    public static async Task<TResult> MatchAsync<TLeft, TRight, TResult>(
        this Task<Either<TLeft, TRight>> eitherTask,
        Func<TLeft, Task<TResult>> leftFuncAsync,
        Func<TRight, Task<TResult>> rightFuncAsync) 
        where TLeft : notnull where TRight : notnull 
    {
        return await (await eitherTask).MatchAsync(leftFuncAsync, rightFuncAsync);
    }
}