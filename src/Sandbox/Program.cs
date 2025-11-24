// See https://aka.ms/new-console-template for more information

using Cysharp.Threading.Tasks;
using HCommons.Monads;
using HCommons.UniTask.Monads;

namespace Sandbox;

public class TestClass {
    public static async UniTask<Optional<int>> Test() {
        var optional = Optional<int>.Some(5);
        var result = await optional.SelectAsyncUniTask(async x => {
            await UniTask.Yield();
            return x * 2;
        }).BindAsyncUniTask(async x => {;
            await UniTask.Yield();
            return Optional<int>.Some(x + 3);
        }).MatchAsyncUniTask(
            async some => {
                await UniTask.Yield();
                return Optional<int>.Some(some);
            },
            async () => {
                await UniTask.Yield();
                return Optional<int>.None();
            }
        );
        
        return result;
    }
}