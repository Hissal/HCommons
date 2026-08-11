using System.ComponentModel;

namespace HCommons.Reflection;

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
