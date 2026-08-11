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
