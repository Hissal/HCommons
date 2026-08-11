using System.ComponentModel;

namespace HCommons.Reflection;

/// <summary>
/// Requests generated <see cref="RuntimeTypeCache"/> catalogs for the annotated base class or interface.
/// </summary>
/// <remarks>
/// Apply this attribute to shared contracts when implementations can be compiled in other assemblies.
/// Direct calls using a concrete generic argument or <see langword="typeof"/> expression are discovered
/// automatically and do not require this attribute.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
public sealed class GenerateRuntimeTypeCacheAttribute : Attribute;

/// <summary>
/// Identifies where a source-generation-aware method obtains its queried base type.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public enum RuntimeTypeCacheQuerySource {
    /// <summary>The base type is a generic type argument.</summary>
    GenericTypeArgument,

    /// <summary>The base type is supplied by a method argument containing a <see langword="typeof"/> expression.</summary>
    MethodArgument,
}

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

/// <summary>
/// Carries a generated type catalog from a compiled assembly to <see cref="RuntimeTypeCache"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
[EditorBrowsable(EditorBrowsableState.Never)]
public sealed class RuntimeTypeCacheGeneratedTypesAttribute : Attribute {
    /// <summary>Creates a generated type catalog entry.</summary>
    public RuntimeTypeCacheGeneratedTypesAttribute(
        Type baseType,
        bool isComplete,
        params Type[] derivedTypes) {
        BaseType = baseType;
        IsComplete = isComplete;
        DerivedTypes = derivedTypes;
    }

    /// <summary>Gets the queried base type.</summary>
    public Type BaseType { get; }

    /// <summary>Gets whether the entry completely describes the declaring assembly for this query.</summary>
    public bool IsComplete { get; }

    /// <summary>Gets the generated matching types.</summary>
    public Type[] DerivedTypes { get; }
}
