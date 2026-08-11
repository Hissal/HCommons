using System.Diagnostics;
using System.Reflection;

namespace HCommons.Reflection;

/// <summary>
/// Discovers and caches types from the assemblies loaded in the current application domain.
/// </summary>
/// <remarks>
/// Results are cached per assembly. Assemblies loaded after the first query are scanned
/// incrementally and merged into existing query results. When the bundled source generator
/// supplies a complete catalog for an assembly and base type, that catalog is used instead
/// of enumerating the assembly's types.
/// </remarks>
public static partial class RuntimeTypeCache {
    static readonly object s_gate = new();
    static readonly Dictionary<Assembly, AssemblyEntry> s_assemblies = new();
    static readonly Dictionary<Type, QueryEntry> s_queries = new();
    static readonly HashSet<Assembly> s_pendingAssemblies = new();

    static bool s_initialized;
    static bool s_snapshotRebuildPending;
    static bool s_workerScheduled;

    static RuntimeTypeCache() {
        AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoad;
    }

    /// <summary>
    /// Returns every loaded type assignable to <typeparamref name="TBase"/>, excluding
    /// <typeparamref name="TBase"/> itself.
    /// </summary>
    /// <typeparam name="TBase">The base class or interface to query.</typeparam>
    /// <returns>An immutable snapshot of the matching types. Result order is unspecified.</returns>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IReadOnlyList<Type> TypesDerivedFrom<TBase>() => TypesDerivedFrom(typeof(TBase));

    /// <summary>
    /// Returns every loaded type assignable to <typeparamref name="TBase"/> that satisfies
    /// <paramref name="filter"/>, excluding <typeparamref name="TBase"/> itself.
    /// </summary>
    /// <typeparam name="TBase">The base class or interface to query.</typeparam>
    /// <param name="filter">A predicate that selects types from the cached query result.</param>
    /// <returns>An immutable snapshot of the matching types. Result order is unspecified.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="filter"/> is <see langword="null"/>.</exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IReadOnlyList<Type> TypesDerivedFrom<TBase>(Func<Type, bool> filter) =>
        TypesDerivedFrom(typeof(TBase), filter);

    /// <summary>
    /// Returns every loaded type assignable to <typeparamref name="TBase"/> that satisfies
    /// <paramref name="filter"/>, excluding <typeparamref name="TBase"/> itself.
    /// </summary>
    /// <typeparam name="TBase">The base class or interface to query.</typeparam>
    /// <param name="filter">A reusable type-filter descriptor.</param>
    /// <returns>An immutable snapshot of the matching types. Result order is unspecified.</returns>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IReadOnlyList<Type> TypesDerivedFrom<TBase>(RuntimeTypeFilter filter) =>
        TypesDerivedFrom(typeof(TBase), filter);

    /// <summary>
    /// Returns every loaded type assignable to <paramref name="baseType"/>, excluding
    /// <paramref name="baseType"/> itself.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <returns>An immutable snapshot of the matching types. Result order is unspecified.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseType"/> is <see langword="null"/>.</exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IReadOnlyList<Type> TypesDerivedFrom(Type baseType) {
        if (baseType is null) {
            throw new ArgumentNullException(nameof(baseType));
        }

        IReadOnlyList<Type> snapshot;
        List<Notification>? notifications;

        lock (s_gate) {
            EnsureInitialized();
            notifications = ProcessPendingAssemblies();
            snapshot = GetOrCreateQuery(baseType).Snapshot;
        }

        Publish(notifications);
        return snapshot;
    }

    /// <summary>
    /// Returns every loaded type assignable to <paramref name="baseType"/> that satisfies
    /// <paramref name="filter"/>, excluding <paramref name="baseType"/> itself.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="filter">A predicate that selects types from the cached query result.</param>
    /// <returns>An immutable snapshot of the matching types. Result order is unspecified.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="baseType"/> or <paramref name="filter"/> is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IReadOnlyList<Type> TypesDerivedFrom(Type baseType, Func<Type, bool> filter) {
        if (baseType is null) {
            throw new ArgumentNullException(nameof(baseType));
        }

        if (filter is null) {
            throw new ArgumentNullException(nameof(filter));
        }

        return FilterSnapshot(TypesDerivedFrom(baseType), filter);
    }

    /// <summary>
    /// Returns every loaded type assignable to <paramref name="baseType"/> that satisfies
    /// <paramref name="filter"/>, excluding <paramref name="baseType"/> itself.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="filter">A reusable type-filter descriptor.</param>
    /// <returns>An immutable snapshot of the matching types. Result order is unspecified.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseType"/> is <see langword="null"/>.</exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IReadOnlyList<Type> TypesDerivedFrom(Type baseType, RuntimeTypeFilter filter) {
        if (baseType is null) {
            throw new ArgumentNullException(nameof(baseType));
        }

        var snapshot = TypesDerivedFrom(baseType);
        return FilterSnapshot(baseType, snapshot, filter);
    }

    /// <summary>
    /// Observes types assignable to <typeparamref name="TBase"/> on the current synchronization context.
    /// </summary>
    /// <param name="onChanged">
    /// A callback that receives the initial snapshot synchronously and later replacement snapshots when the result changes.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IDisposable Bind<TBase>(Action<IReadOnlyList<Type>> onChanged) =>
        Bind(typeof(TBase), onChanged, SynchronizationContext.Current);

    /// <summary>
    /// Observes types assignable to <typeparamref name="TBase"/> that satisfy
    /// <paramref name="filter"/> on the current synchronization context.
    /// </summary>
    /// <param name="filter">A predicate that selects types from each cached query snapshot.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="filter"/> or <paramref name="onChanged"/> is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IDisposable Bind<TBase>(
        Func<Type, bool> filter,
        Action<IReadOnlyList<Type>> onChanged) =>
        Bind(typeof(TBase), filter, onChanged, SynchronizationContext.Current);

    /// <summary>
    /// Observes types assignable to <typeparamref name="TBase"/> that satisfy
    /// <paramref name="filter"/> on the current synchronization context.
    /// </summary>
    /// <param name="filter">A reusable type-filter descriptor.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="onChanged"/> is <see langword="null"/>.</exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IDisposable Bind<TBase>(
        RuntimeTypeFilter filter,
        Action<IReadOnlyList<Type>> onChanged) =>
        Bind(typeof(TBase), filter, onChanged, SynchronizationContext.Current);

    /// <summary>
    /// Observes types assignable to <typeparamref name="TBase"/> on a specified synchronization context.
    /// </summary>
    /// <param name="onChanged">
    /// A callback that receives the initial snapshot synchronously and later replacement snapshots when the result changes.
    /// </param>
    /// <param name="synchronizationContext">
    /// The context used for later notifications, or <see langword="null"/> to use the thread pool.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IDisposable Bind<TBase>(
        Action<IReadOnlyList<Type>> onChanged,
        SynchronizationContext? synchronizationContext) =>
        Bind(typeof(TBase), onChanged, synchronizationContext);

    /// <summary>
    /// Observes types assignable to <typeparamref name="TBase"/> that satisfy
    /// <paramref name="filter"/> on a specified synchronization context.
    /// </summary>
    /// <param name="filter">A predicate that selects types from each cached query snapshot.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <param name="synchronizationContext">
    /// The context used for later notifications, or <see langword="null"/> to use the thread pool.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="filter"/> or <paramref name="onChanged"/> is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IDisposable Bind<TBase>(
        Func<Type, bool> filter,
        Action<IReadOnlyList<Type>> onChanged,
        SynchronizationContext? synchronizationContext) =>
        Bind(typeof(TBase), filter, onChanged, synchronizationContext);

    /// <summary>
    /// Observes types assignable to <typeparamref name="TBase"/> that satisfy
    /// <paramref name="filter"/> on a specified synchronization context.
    /// </summary>
    /// <param name="filter">A reusable type-filter descriptor.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <param name="synchronizationContext">
    /// The context used for later notifications, or <see langword="null"/> to use the thread pool.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="onChanged"/> is <see langword="null"/>.</exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.GenericTypeArgument, 0)]
    public static IDisposable Bind<TBase>(
        RuntimeTypeFilter filter,
        Action<IReadOnlyList<Type>> onChanged,
        SynchronizationContext? synchronizationContext) =>
        Bind(typeof(TBase), filter, onChanged, synchronizationContext);

    /// <summary>
    /// Observes types assignable to <paramref name="baseType"/> on the current synchronization context.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial snapshot synchronously and later replacement snapshots when the result changes.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IDisposable Bind(Type baseType, Action<IReadOnlyList<Type>> onChanged) =>
        Bind(baseType, onChanged, SynchronizationContext.Current);

    /// <summary>
    /// Observes types assignable to <paramref name="baseType"/> that satisfy
    /// <paramref name="filter"/> on the current synchronization context.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="filter">A predicate that selects types from each cached query snapshot.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="baseType"/>, <paramref name="filter"/>, or <paramref name="onChanged"/>
    /// is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IDisposable Bind(
        Type baseType,
        Func<Type, bool> filter,
        Action<IReadOnlyList<Type>> onChanged) =>
        Bind(baseType, filter, onChanged, SynchronizationContext.Current);

    /// <summary>
    /// Observes types assignable to <paramref name="baseType"/> that satisfy
    /// <paramref name="filter"/> on the current synchronization context.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="filter">A reusable type-filter descriptor.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="baseType"/> or <paramref name="onChanged"/> is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IDisposable Bind(
        Type baseType,
        RuntimeTypeFilter filter,
        Action<IReadOnlyList<Type>> onChanged) =>
        Bind(baseType, filter, onChanged, SynchronizationContext.Current);

    /// <summary>
    /// Observes types assignable to <paramref name="baseType"/> on a specified synchronization context.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial snapshot synchronously and later replacement snapshots when the result changes.
    /// </param>
    /// <param name="synchronizationContext">
    /// The context used for later notifications, or <see langword="null"/> to use the thread pool.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="baseType"/> or <paramref name="onChanged"/> is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IDisposable Bind(
        Type baseType,
        Action<IReadOnlyList<Type>> onChanged,
        SynchronizationContext? synchronizationContext)
    {
        if (baseType is null)
            throw new ArgumentNullException(nameof(baseType));

        if (onChanged is null)
            throw new ArgumentNullException(nameof(onChanged));

        Binding binding;
        IReadOnlyList<Type> snapshot;
        List<Notification>? notifications;

        lock (s_gate) {
            EnsureInitialized();
            notifications = ProcessPendingAssemblies();

            var query = GetOrCreateQuery(baseType);
            binding = new Binding(baseType, onChanged, synchronizationContext);
            query.Bindings.Add(binding);
            snapshot = query.Snapshot;
        }

        Publish(notifications);
        binding.Start(snapshot);
        return binding;
    }

    /// <summary>
    /// Observes types assignable to <paramref name="baseType"/> that satisfy
    /// <paramref name="filter"/> on a specified synchronization context.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="filter">A predicate that selects types from each cached query snapshot.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <param name="synchronizationContext">
    /// The context used for later notifications, or <see langword="null"/> to use the thread pool.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="baseType"/>, <paramref name="filter"/>, or <paramref name="onChanged"/>
    /// is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IDisposable Bind(
        Type baseType,
        Func<Type, bool> filter,
        Action<IReadOnlyList<Type>> onChanged,
        SynchronizationContext? synchronizationContext) {
        if (baseType is null) {
            throw new ArgumentNullException(nameof(baseType));
        }

        if (filter is null) {
            throw new ArgumentNullException(nameof(filter));
        }

        if (onChanged is null) {
            throw new ArgumentNullException(nameof(onChanged));
        }

        var observer = new FilteredObserver(filter, onChanged);
        return Bind(baseType, observer.OnChanged, synchronizationContext);
    }

    /// <summary>
    /// Observes types assignable to <paramref name="baseType"/> that satisfy
    /// <paramref name="filter"/> on a specified synchronization context.
    /// </summary>
    /// <param name="baseType">The base class or interface to query.</param>
    /// <param name="filter">A reusable type-filter descriptor.</param>
    /// <param name="onChanged">
    /// A callback that receives the initial filtered snapshot synchronously and later replacement
    /// snapshots when the filtered result changes.
    /// </param>
    /// <param name="synchronizationContext">
    /// The context used for later notifications, or <see langword="null"/> to use the thread pool.
    /// </param>
    /// <returns>A subscription that stops future notifications when disposed.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="baseType"/> or <paramref name="onChanged"/> is <see langword="null"/>.
    /// </exception>
#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "Runtime type discovery cannot guarantee that types have been preserved by trimming.")]
#endif
    [RuntimeTypeCacheSourceGenerationTarget(RuntimeTypeCacheQuerySource.MethodArgument, 0)]
    public static IDisposable Bind(
        Type baseType,
        RuntimeTypeFilter filter,
        Action<IReadOnlyList<Type>> onChanged,
        SynchronizationContext? synchronizationContext) {
        if (baseType is null) {
            throw new ArgumentNullException(nameof(baseType));
        }

        if (onChanged is null) {
            throw new ArgumentNullException(nameof(onChanged));
        }

        var observer = new FilteredObserver(baseType, filter, onChanged);
        return Bind(baseType, observer.OnChanged, synchronizationContext);
    }

    /// <summary>
    /// Invalidates all scanned assemblies and cached query results.
    /// </summary>
    /// <remarks>
    /// Active bindings remain registered. When bindings exist, rebuilding is scheduled in the
    /// background; otherwise it is deferred until the next query or binding.
    /// </remarks>
    public static void Clear() {
        lock (s_gate) {
            s_assemblies.Clear();
            s_pendingAssemblies.Clear();

            foreach (var query in s_queries.Values) {
                query.BeginRebuild();
            }

            s_snapshotRebuildPending = s_queries.Count > 0;

            s_initialized = true;
            QueueCurrentAssemblies();

            if (HasBindings()) {
                ScheduleWorker();
            }
        }
    }

    static void OnAssemblyLoad(object? sender, AssemblyLoadEventArgs args) {
        lock (s_gate) {
            if (!s_assemblies.ContainsKey(args.LoadedAssembly)) {
                s_pendingAssemblies.Add(args.LoadedAssembly);
            }

            if (HasBindings()) {
                ScheduleWorker();
            }
        }
    }

    static void EnsureInitialized() {
        if (s_initialized) {
            return;
        }

        s_initialized = true;
        QueueCurrentAssemblies();
    }

    static void QueueCurrentAssemblies() {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
            if (!s_assemblies.ContainsKey(assembly)) {
                s_pendingAssemblies.Add(assembly);
            }
        }
    }

    static List<Notification>? ProcessPendingAssemblies() {
        if (s_pendingAssemblies.Count == 0 && !s_snapshotRebuildPending) {
            return null;
        }

        var changedQueries = new HashSet<QueryEntry>();

        while (s_pendingAssemblies.Count > 0) {
            var enumerator = s_pendingAssemblies.GetEnumerator();
            enumerator.MoveNext();
            var assembly = enumerator.Current;
            enumerator.Dispose();
            s_pendingAssemblies.Remove(assembly);

            if (s_assemblies.ContainsKey(assembly)) {
                continue;
            }

            var assemblyEntry = new AssemblyEntry(assembly);
            s_assemblies.Add(assembly, assemblyEntry);

            foreach (var query in s_queries.Values) {
                if (query.AddMatches(assemblyEntry.GetTypesFor(query.BaseType))) {
                    changedQueries.Add(query);
                }
            }
        }

        foreach (var query in s_queries.Values) {
            if (query.RequiresSnapshotRebuild) {
                changedQueries.Add(query);
            }
        }

        s_snapshotRebuildPending = false;

        List<Notification>? notifications = null;

        foreach (var query in changedQueries) {
            var changed = query.PublishSnapshot();
            if (!changed) {
                continue;
            }

            foreach (var binding in query.Bindings) {
                notifications ??= new List<Notification>();
                notifications.Add(new Notification(binding, query.Snapshot));
            }
        }

        return notifications;
    }

    static QueryEntry GetOrCreateQuery(Type baseType) {
        if (s_queries.TryGetValue(baseType, out var query)) {
            return query;
        }

        query = new QueryEntry(baseType);
        foreach (var assemblyEntry in s_assemblies.Values) {
            query.AddMatches(assemblyEntry.GetTypesFor(baseType));
        }

        query.PublishInitialSnapshot();
        s_queries.Add(baseType, query);
        return query;
    }

#if NET5_0_OR_GREATER
    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(
        "Trimming",
        "IL2026",
        Justification = "The public discovery APIs warn callers, and Unity consumers are documented to preserve discovered types.")]
#endif
    static Type[] GetLoadableTypes(Assembly assembly) {
        try {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception) {
            return exception.Types.Where(type => type is not null).Cast<Type>().ToArray();
        }
        catch (Exception exception) when (exception is not OutOfMemoryException) {
            Trace.TraceWarning("Unable to scan assembly '{0}' for types: {1}", assembly.FullName, exception);
            return Array.Empty<Type>();
        }
    }

    static bool HasBindings() {
        foreach (var query in s_queries.Values) {
            if (query.Bindings.Count > 0) {
                return true;
            }
        }

        return false;
    }

    static void ScheduleWorker() {
        if (s_workerScheduled) {
            return;
        }

        s_workerScheduled = true;

        try {
            if (!ThreadPool.QueueUserWorkItem(_ => ProcessPendingAssembliesOnWorker())) {
                s_workerScheduled = false;
                Trace.TraceWarning("Unable to queue the runtime type cache worker.");
            }
        }
        catch (Exception exception) when (exception is not OutOfMemoryException) {
            s_workerScheduled = false;
            Trace.TraceWarning("Unable to queue the runtime type cache worker: {0}", exception);
        }
    }

    static void ProcessPendingAssembliesOnWorker() {
        List<Notification>? notifications;

        lock (s_gate) {
            notifications = ProcessPendingAssemblies();
            s_workerScheduled = false;
        }

        Publish(notifications);
    }

    static void Publish(List<Notification>? notifications) {
        if (notifications is null) {
            return;
        }

        foreach (var notification in notifications) {
            notification.Binding.Queue(notification.Snapshot);
        }
    }

    static IReadOnlyList<Type> FilterSnapshot(
        IReadOnlyList<Type> snapshot,
        Func<Type, bool> filter) {
        List<Type>? filteredTypes = null;

        for (var index = 0; index < snapshot.Count; index++) {
            var type = snapshot[index];
            if (filter(type)) {
                filteredTypes?.Add(type);
                continue;
            }

            if (filteredTypes is not null) {
                continue;
            }

            filteredTypes = new List<Type>(snapshot.Count - 1);
            for (var includedIndex = 0; includedIndex < index; includedIndex++) {
                filteredTypes.Add(snapshot[includedIndex]);
            }
        }

        return filteredTypes is null
            ? snapshot
            : Array.AsReadOnly(filteredTypes.ToArray());
    }

    static IReadOnlyList<Type> FilterSnapshot(
        Type baseType,
        IReadOnlyList<Type> snapshot,
        RuntimeTypeFilter filter) {
        if (!filter.IsCacheable) {
            return FilterSnapshot(snapshot, filter);
        }

        QueryEntry? query = null;
        FilteredSnapshotEntry? cacheEntry = null;

        lock (s_gate) {
            if (s_queries.TryGetValue(baseType, out query)) {
                if (query.FilteredSnapshots.TryGetValue(filter, out cacheEntry)) {
                    if (ReferenceEquals(cacheEntry.SourceSnapshot, snapshot) &&
                        cacheEntry.FilteredSnapshot is not null) {
                        return cacheEntry.FilteredSnapshot;
                    }
                }
                else if (filter.CacheRequested) {
                    cacheEntry = new FilteredSnapshotEntry();
                    query.FilteredSnapshots.Add(filter, cacheEntry);
                }
            }
        }

        var filteredSnapshot = FilterSnapshot(snapshot, filter);
        if (query is null || cacheEntry is null) {
            return filteredSnapshot;
        }

        lock (s_gate) {
            if (!ReferenceEquals(query.Snapshot, snapshot)) {
                return filteredSnapshot;
            }

            if (query.FilteredSnapshots.TryGetValue(filter, out var currentEntry)) {
                if (ReferenceEquals(currentEntry.SourceSnapshot, snapshot) &&
                    currentEntry.FilteredSnapshot is not null) {
                    return currentEntry.FilteredSnapshot;
                }

                currentEntry.SourceSnapshot = snapshot;
                currentEntry.FilteredSnapshot = filteredSnapshot;
            }
            else if (filter.CacheRequested) {
                query.FilteredSnapshots.Add(
                    filter,
                    new FilteredSnapshotEntry(snapshot, filteredSnapshot));
            }
        }

        return filteredSnapshot;
    }

    static IReadOnlyList<Type> FilterSnapshot(
        IReadOnlyList<Type> snapshot,
        RuntimeTypeFilter filter) {
        List<Type>? filteredTypes = null;

        for (var index = 0; index < snapshot.Count; index++) {
            var type = snapshot[index];
            if (filter.MatchesUnchecked(type)) {
                filteredTypes?.Add(type);
                continue;
            }

            if (filteredTypes is not null) {
                continue;
            }

            filteredTypes = new List<Type>(snapshot.Count - 1);
            for (var includedIndex = 0; includedIndex < index; includedIndex++) {
                filteredTypes.Add(snapshot[includedIndex]);
            }
        }

        return filteredTypes is null
            ? snapshot
            : Array.AsReadOnly(filteredTypes.ToArray());
    }

    static void Unbind(Binding binding) {
        lock (s_gate) {
            if (s_queries.TryGetValue(binding.BaseType, out var query)) {
                query.Bindings.Remove(binding);
            }
        }
    }
}
