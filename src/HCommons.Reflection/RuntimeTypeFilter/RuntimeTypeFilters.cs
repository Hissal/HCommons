namespace HCommons.Reflection;

/// <summary>
/// Creates reusable <see cref="RuntimeTypeFilter"/> values.
/// </summary>
public static class RuntimeTypeFilters {
    /// <summary>Matches types that are neither abstract nor interfaces.</summary>
    public static RuntimeTypeFilter Concrete() => new(RuntimeTypeFilterFlags.Concrete);

    /// <summary>Matches externally visible types.</summary>
    public static RuntimeTypeFilter Public() => new(RuntimeTypeFilterFlags.Public);

    /// <summary>Matches types that contain no unassigned generic parameters.</summary>
    public static RuntimeTypeFilter Closed() => new(RuntimeTypeFilterFlags.Closed);

    /// <summary>Matches value types and types with a public parameterless constructor.</summary>
    public static RuntimeTypeFilter HasPublicParameterlessConstructor() =>
        new(RuntimeTypeFilterFlags.PublicParameterlessConstructor);

    /// <summary>
    /// Matches concrete, closed types that are value types or have a public parameterless constructor.
    /// </summary>
    public static RuntimeTypeFilter Instantiable() => new(
        RuntimeTypeFilterFlags.Concrete |
        RuntimeTypeFilterFlags.Closed |
        RuntimeTypeFilterFlags.PublicParameterlessConstructor);

    /// <summary>Creates an uncacheable filter from an arbitrary predicate.</summary>
    public static RuntimeTypeFilter Where(Func<Type, bool> predicate) =>
        default(RuntimeTypeFilter).Where(predicate);

    /// <summary>Creates an uncacheable filter from explicitly supplied state and a predicate.</summary>
    public static RuntimeTypeFilter Where<TState>(
        TState state,
        Func<TState, Type, bool> predicate) =>
        default(RuntimeTypeFilter).Where(state, predicate);

    /// <summary>Creates a structurally cacheable filter from an immutable record rule.</summary>
    public static RuntimeTypeFilter Where(RuntimeTypeFilterRule rule) =>
        default(RuntimeTypeFilter).Where(rule);

    /// <summary>Negates the complete supplied filter expression.</summary>
    public static RuntimeTypeFilter Not(RuntimeTypeFilter filter) => filter.Negated();
}
