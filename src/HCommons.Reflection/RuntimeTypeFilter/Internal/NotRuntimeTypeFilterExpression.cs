namespace HCommons.Reflection;

internal sealed record NotRuntimeTypeFilterExpression(RuntimeTypeFilter Operand) : RuntimeTypeFilterExpression {
    public override bool IsCacheable => Operand.IsCacheable;

    public override bool Matches(Type type) => !Operand.MatchesUnchecked(type);
}
