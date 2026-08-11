namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    sealed class GeneratedQueryBuilder {
        public bool IsComplete { get; set; } = true;

        public HashSet<Type> Types { get; } = new();
    }
}
