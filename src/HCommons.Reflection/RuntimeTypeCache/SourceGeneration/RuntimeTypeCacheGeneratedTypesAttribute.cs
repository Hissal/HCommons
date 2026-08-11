using System.ComponentModel;

namespace HCommons.Reflection;

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
