namespace HCommons.Reflection;

public static partial class RuntimeTypeCache {
    readonly record struct Notification(Binding Binding, IReadOnlyList<Type> Snapshot);
}
