namespace HCommons.Void;

/// <summary>
/// Represents a type that has no value or state. This is useful in scenarios where a type is required
/// but no meaningful value needs to be conveyed, such as in generic constraints or as a placeholder.
/// </summary>
public readonly record struct Void {
    /// <summary>
    /// Gets the singleton instance of the <see cref="Void"/> type.
    /// </summary>
    public static Void Value => new Void();
}