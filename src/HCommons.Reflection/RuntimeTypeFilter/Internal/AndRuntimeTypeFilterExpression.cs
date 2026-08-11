namespace HCommons.Reflection;

internal sealed record AndRuntimeTypeFilterExpression(
    RuntimeTypeFilter Left,
    RuntimeTypeFilter Right) : RuntimeTypeFilterExpression {
    public override bool IsCacheable => Left.IsCacheable && Right.IsCacheable;

    public override bool Matches(Type type) => Left.MatchesUnchecked(type) && Right.MatchesUnchecked(type);
}
