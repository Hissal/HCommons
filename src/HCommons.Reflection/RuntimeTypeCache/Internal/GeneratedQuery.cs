namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    readonly record struct GeneratedQuery(bool IsComplete, Type[] Types);
}
