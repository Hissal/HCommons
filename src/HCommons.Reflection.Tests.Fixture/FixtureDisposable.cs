using HCommons.Reflection;

namespace HCommons.Reflection.Tests.Fixture;

public sealed class FixtureDisposable : IDisposable {
    public void Dispose() { }

    public static IReadOnlyList<Type> DiscoverTypes() => RuntimeTypeCache.TypesDerivedFrom<IDisposable>();
}
