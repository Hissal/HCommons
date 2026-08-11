using BenchmarkDotNet.Attributes;
using HCommons.Reflection;

[MemoryDiagnoser]
public sealed class RuntimeTypeFilterBench {
    readonly RuntimeTypeFilterBenchmarkState _state = new(typeof(IDisposable));

    [Benchmark(Baseline = true)]
    public RuntimeTypeFilter CapturingWhere() =>
        RuntimeTypeFilters.Where(type => _state.Matches(type));

    [Benchmark]
    public RuntimeTypeFilter StatefulWhere() =>
        RuntimeTypeFilters.Where(
            _state,
            static (state, type) => state.Matches(type));
}
