namespace HCommons.Reflection;

/// <summary>
/// Describes a reusable predicate over runtime <see cref="Type"/> instances.
/// </summary>
/// <remarks>
/// Built-in filters are stored as flags and can be composed without heap allocations. Compound
/// Boolean expressions and custom rules use immutable expression nodes. The default value matches
/// every non-null type.
/// </remarks>
public readonly struct RuntimeTypeFilter : IEquatable<RuntimeTypeFilter> {
    readonly RuntimeTypeFilterFlags _flags;
    readonly RuntimeTypeFilterExpression? _expression;
    readonly bool _cacheRequested;

    internal RuntimeTypeFilter(RuntimeTypeFilterFlags flags) : this(flags, null, false) { }

    RuntimeTypeFilter(
        RuntimeTypeFilterFlags flags,
        RuntimeTypeFilterExpression? expression,
        bool cacheRequested) {
        _flags = flags;
        _expression = expression;
        _cacheRequested = cacheRequested;
    }

    /// <summary>
    /// Gets whether this filter has a stable structural identity and can safely cache result snapshots.
    /// </summary>
    public bool IsCacheable => _expression?.IsCacheable ?? true;

    internal bool CacheRequested => _cacheRequested;

    /// <summary>
    /// Returns whether <paramref name="type"/> satisfies this filter.
    /// </summary>
    /// <param name="type">The type to evaluate.</param>
    /// <exception cref="ArgumentNullException"><paramref name="type"/> is <see langword="null"/>.</exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type filters can inspect reflected type metadata that may be removed by trimming.")]
#endif
    public bool Matches(Type type) {
        if (type is null) {
            throw new ArgumentNullException(nameof(type));
        }

        return MatchesUnchecked(type);
    }

    /// <summary>
    /// Adds the built-in concrete-type condition to this filter.
    /// </summary>
    public RuntimeTypeFilter Concrete() => And(RuntimeTypeFilters.Concrete());

    /// <summary>
    /// Adds the built-in externally-visible-type condition to this filter.
    /// </summary>
    public RuntimeTypeFilter Public() => And(RuntimeTypeFilters.Public());

    /// <summary>
    /// Adds the built-in closed-type condition to this filter.
    /// </summary>
    public RuntimeTypeFilter Closed() => And(RuntimeTypeFilters.Closed());

    /// <summary>
    /// Adds the condition that a type is a value type or has a public parameterless constructor.
    /// </summary>
    public RuntimeTypeFilter HasPublicParameterlessConstructor() =>
        And(RuntimeTypeFilters.HasPublicParameterlessConstructor());

    /// <summary>
    /// Adds the built-in instantiable-type conditions to this filter.
    /// </summary>
    /// <remarks>
    /// An instantiable type is concrete, closed, and either a value type or has a public
    /// parameterless constructor. External visibility is a separate condition; append
    /// <see cref="Public"/> when it is required.
    /// </remarks>
    public RuntimeTypeFilter Instantiable() => And(RuntimeTypeFilters.Instantiable());

    /// <summary>
    /// Adds an arbitrary, uncacheable predicate to this filter.
    /// </summary>
    /// <param name="predicate">The predicate to evaluate after the accumulated filter matches.</param>
    /// <remarks>
    /// Delegate behavior has no stable structural identity. A filter containing this condition
    /// remains usable but ignores a request made through <see cref="Cached"/>.
    /// </remarks>
    public RuntimeTypeFilter Where(Func<Type, bool> predicate) {
        if (predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        return And(new RuntimeTypeFilter(
            RuntimeTypeFilterFlags.None,
            new DelegateRuntimeTypeFilterExpression(predicate),
            cacheRequested: false));
    }

    /// <summary>
    /// Adds an arbitrary, uncacheable predicate with explicitly supplied state to this filter.
    /// </summary>
    /// <typeparam name="TState">The type of state passed to the predicate.</typeparam>
    /// <param name="state">The state stored with the filter expression.</param>
    /// <param name="predicate">The predicate that evaluates the state and candidate type.</param>
    /// <remarks>
    /// Use a static lambda to prevent accidental captures. Value-type state is stored directly in
    /// the generic expression node without boxing. Stateful delegate behavior has no stable
    /// structural identity, so a filter containing this condition ignores <see cref="Cached"/>.
    /// </remarks>
    public RuntimeTypeFilter Where<TState>(
        TState state,
        Func<TState, Type, bool> predicate) {
        if (predicate is null) {
            throw new ArgumentNullException(nameof(predicate));
        }

        return And(new RuntimeTypeFilter(
            RuntimeTypeFilterFlags.None,
            new StatefulDelegateRuntimeTypeFilterExpression<TState>(state, predicate),
            cacheRequested: false));
    }

    /// <summary>
    /// Adds a structurally cacheable rule to this filter.
    /// </summary>
    /// <param name="rule">An immutable rule whose record equality represents all matching behavior.</param>
    public RuntimeTypeFilter Where(RuntimeTypeFilterRule rule) {
        if (rule is null) {
            throw new ArgumentNullException(nameof(rule));
        }

        return And(new RuntimeTypeFilter(
            RuntimeTypeFilterFlags.None,
            new RuleRuntimeTypeFilterExpression(rule),
            cacheRequested: false));
    }

    /// <summary>
    /// Combines the accumulated expression and <paramref name="other"/> with Boolean AND.
    /// </summary>
    public RuntimeTypeFilter And(RuntimeTypeFilter other) {
        var cacheRequested = _cacheRequested || other._cacheRequested;

        if (IsMatchAll) {
            return other.WithCacheRequest(cacheRequested);
        }

        if (other.IsMatchAll) {
            return WithCacheRequest(cacheRequested);
        }

        if (_expression is null && other._expression is null) {
            return new RuntimeTypeFilter(_flags | other._flags, null, cacheRequested);
        }

        return new RuntimeTypeFilter(
            RuntimeTypeFilterFlags.None,
            new AndRuntimeTypeFilterExpression(this, other),
            cacheRequested);
    }

    /// <summary>
    /// Combines the accumulated expression and <paramref name="other"/> with Boolean OR.
    /// </summary>
    public RuntimeTypeFilter Or(RuntimeTypeFilter other) {
        var cacheRequested = _cacheRequested || other._cacheRequested;

        if (IsMatchAll) {
            return new RuntimeTypeFilter(RuntimeTypeFilterFlags.None, null, cacheRequested);
        }

        return new RuntimeTypeFilter(
            RuntimeTypeFilterFlags.None,
            new OrRuntimeTypeFilterExpression(this, other),
            cacheRequested);
    }

    /// <summary>
    /// Adds the negation of <paramref name="other"/> to this filter with Boolean AND.
    /// </summary>
    public RuntimeTypeFilter Not(RuntimeTypeFilter other) => And(RuntimeTypeFilters.Not(other));

    /// <summary>
    /// Requests snapshot caching when this filter is used by <see cref="RuntimeTypeCache"/>.
    /// </summary>
    /// <remarks>
    /// Caching is ignored when <see cref="IsCacheable"/> is <see langword="false"/>. Calling
    /// <see cref="Matches"/> directly always evaluates the filter and does not use a snapshot cache.
    /// </remarks>
    public RuntimeTypeFilter Cached() => WithCacheRequest(cacheRequested: true);

    /// <inheritdoc/>
    public bool Equals(RuntimeTypeFilter other) =>
        _flags == other._flags && Equals(_expression, other._expression);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is RuntimeTypeFilter other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() {
        unchecked {
            return ((int)_flags * 397) ^ (_expression?.GetHashCode() ?? 0);
        }
    }

    /// <summary>Returns whether two filters have the same matching expression.</summary>
    public static bool operator ==(RuntimeTypeFilter left, RuntimeTypeFilter right) => left.Equals(right);

    /// <summary>Returns whether two filters have different matching expressions.</summary>
    public static bool operator !=(RuntimeTypeFilter left, RuntimeTypeFilter right) => !left.Equals(right);

    internal RuntimeTypeFilter Negated() => new(
        RuntimeTypeFilterFlags.None,
        new NotRuntimeTypeFilterExpression(this),
        _cacheRequested);

    internal bool MatchesUnchecked(Type type) =>
        _expression?.Matches(type) ?? MatchesFlags(type, _flags);

    bool IsMatchAll => _flags == RuntimeTypeFilterFlags.None && _expression is null;

    RuntimeTypeFilter WithCacheRequest(bool cacheRequested) =>
        new(_flags, _expression, cacheRequested);

#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(
        "Trimming",
        "IL2070",
        Justification = "The public Matches and RuntimeTypeCache APIs warn callers about reflected metadata.")]
#endif
    internal static bool MatchesFlags(Type type, RuntimeTypeFilterFlags flags) {
        if ((flags & RuntimeTypeFilterFlags.Concrete) != 0 && (type.IsAbstract || type.IsInterface)) {
            return false;
        }

        if ((flags & RuntimeTypeFilterFlags.Public) != 0 && !type.IsVisible) {
            return false;
        }

        if ((flags & RuntimeTypeFilterFlags.Closed) != 0 && type.ContainsGenericParameters) {
            return false;
        }

        if ((flags & RuntimeTypeFilterFlags.PublicParameterlessConstructor) != 0 &&
            !type.IsValueType &&
            type.GetConstructor(Type.EmptyTypes) is null) {
            return false;
        }

        return true;
    }
}
