namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    sealed class QueryEntry {
        readonly Type _baseType;
        HashSet<Type>? _preRebuildTypes;

        public QueryEntry(Type baseType) {
            _baseType = baseType;
        }

        public Type BaseType => _baseType;

        public HashSet<Type> Types { get; } = new();

        public IReadOnlyList<Type> Snapshot { get; private set; } = Array.AsReadOnly(Array.Empty<Type>());

        public List<Binding> Bindings { get; } = new();

        public Dictionary<RuntimeTypeFilter, FilteredSnapshotEntry> FilteredSnapshots { get; } = new();

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
            FilteredSnapshots.Clear();
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
}
