namespace HCommons.Reflection;

internal abstract record RuntimeTypeFilterExpression {
    public abstract bool IsCacheable { get; }

    public abstract bool Matches(Type type);
}
