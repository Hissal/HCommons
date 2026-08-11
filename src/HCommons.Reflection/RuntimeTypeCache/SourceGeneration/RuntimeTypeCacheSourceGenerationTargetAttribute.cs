using System.ComponentModel;

namespace HCommons.Reflection;

/// <summary>
/// Marks a <see cref="RuntimeTypeCache"/> method as a source-generator query entry point.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RuntimeTypeCacheSourceGenerationTargetAttribute : Attribute {
    /// <summary>Creates a generator target marker.</summary>
    public RuntimeTypeCacheSourceGenerationTargetAttribute(RuntimeTypeCacheQuerySource source, int index) {
        Source = source;
        Index = index;
    }

    /// <summary>Gets the location from which the base type is read.</summary>
    public RuntimeTypeCacheQuerySource Source { get; }

    /// <summary>Gets the zero-based generic type argument or method parameter index.</summary>
    public int Index { get; }
}
