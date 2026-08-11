using System.Diagnostics;
using System.Reflection;

namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    sealed class AssemblyEntry {
        readonly Assembly _assembly;
        readonly Dictionary<Type, GeneratedQuery> _generatedQueries;

        Type[]? _reflectedTypes;

        public AssemblyEntry(Assembly assembly) {
            _assembly = assembly;
            _generatedQueries = ReadGeneratedQueries(assembly);
        }

        public Type[] GetTypesFor(Type baseType) {
            if (_generatedQueries.TryGetValue(baseType, out var generatedQuery) && generatedQuery.IsComplete) {
                return generatedQuery.Types;
            }

            return _reflectedTypes ??= GetLoadableTypes(_assembly);
        }

        static Dictionary<Type, GeneratedQuery> ReadGeneratedQueries(Assembly assembly) {
            object[] attributes;

            try {
                attributes = assembly.GetCustomAttributes(
                    typeof(RuntimeTypeCacheGeneratedTypesAttribute),
                    inherit: false);
            }
            catch (Exception exception) when (exception is not OutOfMemoryException) {
                Trace.TraceWarning(
                    "Unable to read generated runtime type catalogs from assembly '{0}': {1}",
                    assembly.FullName,
                    exception);
                return new Dictionary<Type, GeneratedQuery>();
            }

            var builders = new Dictionary<Type, GeneratedQueryBuilder>();

            foreach (var attribute in attributes.Cast<RuntimeTypeCacheGeneratedTypesAttribute>()) {
                if (attribute.BaseType is null) {
                    continue;
                }

                if (!builders.TryGetValue(attribute.BaseType, out var builder)) {
                    builder = new GeneratedQueryBuilder();
                    builders.Add(attribute.BaseType, builder);
                }

                builder.IsComplete &= attribute.IsComplete;

                if (attribute.DerivedTypes is null) {
                    builder.IsComplete = false;
                    continue;
                }

                foreach (var type in attribute.DerivedTypes) {
                    if (type is null || type == attribute.BaseType || !attribute.BaseType.IsAssignableFrom(type)) {
                        builder.IsComplete = false;
                        continue;
                    }

                    builder.Types.Add(type);
                }
            }

            var queries = new Dictionary<Type, GeneratedQuery>(builders.Count);
            foreach (var pair in builders) {
                queries.Add(pair.Key, new GeneratedQuery(pair.Value.IsComplete, pair.Value.Types.ToArray()));
            }

            return queries;
        }
    }
}
