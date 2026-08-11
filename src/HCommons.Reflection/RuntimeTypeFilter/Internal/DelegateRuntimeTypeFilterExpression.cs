namespace HCommons.Reflection;

internal sealed record DelegateRuntimeTypeFilterExpression(Func<Type, bool> Predicate) : RuntimeTypeFilterExpression {
    public override bool IsCacheable => false;

    public override bool Matches(Type type) => Predicate(type);
}
