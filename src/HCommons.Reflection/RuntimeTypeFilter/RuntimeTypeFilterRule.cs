namespace HCommons.Reflection;

/// <summary>
/// Defines a reusable, structurally cacheable runtime type-filter rule.
/// </summary>
/// <remarks>
/// Implementations must be immutable records. Every value that can affect <see cref="Matches"/>
/// must participate in record equality, and matching must not depend on mutable external state.
/// </remarks>
public abstract record RuntimeTypeFilterRule {
    /// <summary>Returns whether <paramref name="type"/> satisfies this rule.</summary>
    public abstract bool Matches(Type type);
}
