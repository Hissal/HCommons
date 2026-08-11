namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    sealed class FilteredSnapshotEntry {
        public FilteredSnapshotEntry() { }

        public FilteredSnapshotEntry(
            IReadOnlyList<Type> sourceSnapshot,
            IReadOnlyList<Type> filteredSnapshot) {
            SourceSnapshot = sourceSnapshot;
            FilteredSnapshot = filteredSnapshot;
        }

        public IReadOnlyList<Type>? SourceSnapshot { get; set; }

        public IReadOnlyList<Type>? FilteredSnapshot { get; set; }
    }
}
