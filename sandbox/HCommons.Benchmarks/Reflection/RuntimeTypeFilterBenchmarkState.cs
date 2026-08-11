public readonly struct RuntimeTypeFilterBenchmarkState {
    readonly Type _expected;

    public RuntimeTypeFilterBenchmarkState(Type expected) {
        _expected = expected;
    }

    public bool Matches(Type type) => type == _expected;
}
