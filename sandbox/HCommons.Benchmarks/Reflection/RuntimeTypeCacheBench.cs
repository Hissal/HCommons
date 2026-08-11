using BenchmarkDotNet.Attributes;
using HCommons.Reflection;

[MemoryDiagnoser]
public class RuntimeTypeCacheBench {
    static readonly Func<Type, bool> s_concreteFilter =
        type => !type.IsAbstract && !type.IsInterface;
    static readonly RuntimeTypeFilter s_uncachedDescriptor =
        RuntimeTypeFilters.Concrete().Closed();
    static readonly RuntimeTypeFilter s_cachedDescriptor =
        RuntimeTypeFilters.Concrete().Cached();

    [GlobalSetup]
    public void GlobalSetup() {
        RuntimeTypeCache.Clear();
        RuntimeTypeCache.TypesDerivedFrom<IDisposable>();
        RuntimeTypeCache.TypesDerivedFrom<IGeneratedRuntimeTypeCacheBenchmark>();
        RuntimeTypeCache.TypesDerivedFrom<IDisposable>(s_cachedDescriptor);
    }

    [Benchmark(Baseline = true)]
    public IReadOnlyList<Type> CachedQuery() => RuntimeTypeCache.TypesDerivedFrom<IDisposable>();

    [Benchmark]
    public IReadOnlyList<Type> CachedFilteredQuery() =>
        RuntimeTypeCache.TypesDerivedFrom<IDisposable>(s_concreteFilter);

    [Benchmark]
    public IReadOnlyList<Type> UncachedDescriptorQuery() =>
        RuntimeTypeCache.TypesDerivedFrom<IDisposable>(s_uncachedDescriptor);

    [Benchmark]
    public IReadOnlyList<Type> CachedDescriptorQuery() =>
        RuntimeTypeCache.TypesDerivedFrom<IDisposable>(s_cachedDescriptor);

    [Benchmark]
    public IReadOnlyList<Type> ReflectionFullRebuild() {
        RuntimeTypeCache.Clear();
        return RuntimeTypeCache.TypesDerivedFrom<IDisposable>();
    }

    [Benchmark]
    public IReadOnlyList<Type> CachedGeneratedQuery() =>
        RuntimeTypeCache.TypesDerivedFrom<IGeneratedRuntimeTypeCacheBenchmark>();

    [Benchmark]
    public IReadOnlyList<Type> GeneratedFullRebuild() {
        RuntimeTypeCache.Clear();
        return RuntimeTypeCache.TypesDerivedFrom<IGeneratedRuntimeTypeCacheBenchmark>();
    }
}
