using System.Diagnostics;
using System.Reflection;

namespace HCommons.Reflection;

/// <summary>
/// Discovers and caches types from the assemblies loaded in the current application domain.
/// </summary>
/// <remarks>
/// Results are cached per assembly. Assemblies loaded after the first query are scanned
/// incrementally and merged into existing query results.
/// </remarks>
public static class RuntimeTypeCache {
    static readonly object s_gate = new();
    static readonly Dictionary<Assembly, Type[]> s_assemblyTypes = new();
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
    public static IReadOnlyList<Type> TypesDerivedFrom<TBase>() => TypesDerivedFrom(typeof(TBase));

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
    public static IDisposable Bind<TBase>(Action<IReadOnlyList<Type>> onChanged) =>
        Bind(typeof(TBase), onChanged, SynchronizationContext.Current);

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
    public static IDisposable Bind<TBase>(
        Action<IReadOnlyList<Type>> onChanged,
        SynchronizationContext? synchronizationContext) =>
        Bind(typeof(TBase), onChanged, synchronizationContext);

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
    public static IDisposable Bind(Type baseType, Action<IReadOnlyList<Type>> onChanged) =>
        Bind(baseType, onChanged, SynchronizationContext.Current);

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
    /// Invalidates all scanned assemblies and cached query results.
    /// </summary>
    /// <remarks>
    /// Active bindings remain registered. When bindings exist, rebuilding is scheduled in the
    /// background; otherwise it is deferred until the next query or binding.
    /// </remarks>
    public static void Clear() {
        lock (s_gate) {
            s_assemblyTypes.Clear();
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
            if (!s_assemblyTypes.ContainsKey(args.LoadedAssembly)) {
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
            if (!s_assemblyTypes.ContainsKey(assembly)) {
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

            if (s_assemblyTypes.ContainsKey(assembly)) {
                continue;
            }

            var assemblyTypes = GetLoadableTypes(assembly);
            s_assemblyTypes.Add(assembly, assemblyTypes);

            foreach (var query in s_queries.Values) {
                if (query.AddMatches(assemblyTypes)) {
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
        foreach (var assemblyTypes in s_assemblyTypes.Values) {
            query.AddMatches(assemblyTypes);
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

    static void Unbind(Binding binding) {
        lock (s_gate) {
            if (s_queries.TryGetValue(binding.BaseType, out var query)) {
                query.Bindings.Remove(binding);
            }
        }
    }

    sealed class QueryEntry {
        readonly Type _baseType;
        HashSet<Type>? _preRebuildTypes;

        public QueryEntry(Type baseType) {
            _baseType = baseType;
        }

        public HashSet<Type> Types { get; } = new();

        public IReadOnlyList<Type> Snapshot { get; private set; } = Array.AsReadOnly(Array.Empty<Type>());

        public List<Binding> Bindings { get; } = new();

        public bool RequiresSnapshotRebuild => _preRebuildTypes is not null;

        public bool AddMatches(Type[] types) {
            var changed = false;

            foreach (var type in types) {
                if (type != _baseType && _baseType.IsAssignableFrom(type)) {
                    changed |= Types.Add(type);
                }
            }

            return changed;
        }

        public void BeginRebuild() {
            _preRebuildTypes ??= new HashSet<Type>(Types);
            Types.Clear();
        }

        public void PublishInitialSnapshot() {
            Snapshot = CreateSnapshot();
        }

        public bool PublishSnapshot() {
            var changed = _preRebuildTypes is null || !_preRebuildTypes.SetEquals(Types);
            Snapshot = CreateSnapshot();
            _preRebuildTypes = null;
            return changed;
        }

        IReadOnlyList<Type> CreateSnapshot() => Array.AsReadOnly(Types.ToArray());
    }

    readonly record struct Notification(Binding Binding, IReadOnlyList<Type> Snapshot);

    sealed class Binding : IDisposable {
        readonly object _gate = new();
        readonly Action<IReadOnlyList<Type>> _onChanged;
        readonly SynchronizationContext? _synchronizationContext;

        IReadOnlyList<Type>? _pendingSnapshot;
        bool _started;
        bool _deliveryInProgress;
        bool _dispatchScheduled;
        bool _disposed;

        public Binding(
            Type baseType,
            Action<IReadOnlyList<Type>> onChanged,
            SynchronizationContext? synchronizationContext) {
            BaseType = baseType;
            _onChanged = onChanged;
            _synchronizationContext = synchronizationContext;
        }

        public Type BaseType { get; }

        public void Dispose() {
            lock (_gate) {
                if (_disposed) {
                    return;
                }

                _disposed = true;
                _pendingSnapshot = null;
            }

            Unbind(this);
        }

        public void Start(IReadOnlyList<Type> snapshot) {
            IReadOnlyList<Type>? snapshotToDeliver;

            lock (_gate) {
                if (_disposed) {
                    return;
                }

                _started = true;
                _pendingSnapshot ??= snapshot;
                snapshotToDeliver = _pendingSnapshot;
                _pendingSnapshot = null;
                _deliveryInProgress = true;
            }

            Deliver(snapshotToDeliver);
        }

        public void Queue(IReadOnlyList<Type> snapshot) {
            var shouldSchedule = false;

            lock (_gate) {
                if (_disposed) {
                    return;
                }

                _pendingSnapshot = snapshot;

                if (_started && !_deliveryInProgress && !_dispatchScheduled) {
                    _dispatchScheduled = true;
                    shouldSchedule = true;
                }
            }

            if (shouldSchedule) {
                ScheduleDispatch();
            }
        }

        void ScheduleDispatch() {
            try {
                if (_synchronizationContext is not null) {
                    _synchronizationContext.Post(_ => Dispatch(), null);
                    return;
                }

                if (ThreadPool.QueueUserWorkItem(_ => Dispatch())) {
                    return;
                }

                Trace.TraceWarning("Unable to queue a runtime type cache binding callback.");
            }
            catch (Exception exception) when (exception is not OutOfMemoryException) {
                Trace.TraceWarning("Unable to queue a runtime type cache binding callback: {0}", exception);
            }

            lock (_gate) {
                _dispatchScheduled = false;
            }
        }

        void Dispatch() {
            IReadOnlyList<Type>? snapshot;

            lock (_gate) {
                _dispatchScheduled = false;

                if (_disposed || _pendingSnapshot is null) {
                    return;
                }

                snapshot = _pendingSnapshot;
                _pendingSnapshot = null;
                _deliveryInProgress = true;
            }

            Deliver(snapshot);
        }

        void Deliver(IReadOnlyList<Type> snapshot) {
            try {
                _onChanged(snapshot);
            }
            catch (Exception exception) when (exception is not OutOfMemoryException) {
                Trace.TraceWarning("A runtime type cache binding callback failed: {0}", exception);
            }
            finally {
                CompleteDelivery();
            }
        }

        void CompleteDelivery() {
            var shouldSchedule = false;

            lock (_gate) {
                _deliveryInProgress = false;

                if (!_disposed && _started && _pendingSnapshot is not null && !_dispatchScheduled) {
                    _dispatchScheduled = true;
                    shouldSchedule = true;
                }
            }

            if (shouldSchedule) {
                ScheduleDispatch();
            }
        }
    }
}
