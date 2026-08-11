namespace HCommons.Reflection;

internal sealed record StatefulDelegateRuntimeTypeFilterExpression<TState>(
    TState State,
    Func<TState, Type, bool> Predicate) : RuntimeTypeFilterExpression {
    public override bool IsCacheable => false;

    public override bool Matches(Type type) => Predicate(State, type);
}
