namespace HCommons.Reflection;

internal sealed record RuleRuntimeTypeFilterExpression(RuntimeTypeFilterRule Rule) : RuntimeTypeFilterExpression {
    public override bool IsCacheable => true;

    public override bool Matches(Type type) => Rule.Matches(type);
}
