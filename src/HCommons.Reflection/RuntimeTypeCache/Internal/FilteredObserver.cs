namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    sealed class FilteredObserver {
        readonly Type? _baseType;
        readonly Func<Type, bool>? _filter;
        readonly RuntimeTypeFilter _descriptor;
        readonly Action<IReadOnlyList<Type>> _onChanged;

        HashSet<Type>? _previousTypes;

        public FilteredObserver(
            Func<Type, bool> filter,
            Action<IReadOnlyList<Type>> onChanged) {
            _filter = filter;
            _onChanged = onChanged;
        }

        public FilteredObserver(
            Type baseType,
            RuntimeTypeFilter descriptor,
            Action<IReadOnlyList<Type>> onChanged) {
            _baseType = baseType;
            _descriptor = descriptor;
            _onChanged = onChanged;
        }

        public void OnChanged(IReadOnlyList<Type> snapshot) {
            var filteredSnapshot = _filter is not null
                ? FilterSnapshot(snapshot, _filter)
                : FilterSnapshot(_baseType!, snapshot, _descriptor);
            if (_previousTypes is not null && _previousTypes.SetEquals(filteredSnapshot)) {
                return;
            }

            _previousTypes = new HashSet<Type>(filteredSnapshot);
            _onChanged(filteredSnapshot);
        }
    }
}
