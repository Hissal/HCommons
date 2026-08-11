using System.Reflection;
using HCommons.Reflection;

namespace HCommons.Tests;

[Collection(nameof(NonParallelTests))]
public sealed class RuntimeTypeCacheSourceGenerationTests {
    [Fact]
    public void GenerateAttribute_EmitsACompleteCatalog() {
        var catalog = GetCatalog(typeof(IGeneratedAttributeMarker));

        catalog.IsComplete.ShouldBeTrue();
        catalog.DerivedTypes.ShouldContain(typeof(GeneratedAttributeMarkerBase));
        catalog.DerivedTypes.ShouldContain(typeof(GeneratedAttributeMarkerImplementation));
    }

    [Fact]
    public void GenericCall_EmitsACompleteCatalog() {
        _ = RuntimeTypeCache.TypesDerivedFrom<IGeneratedCallMarker>();

        var catalog = GetCatalog(typeof(IGeneratedCallMarker));

        catalog.IsComplete.ShouldBeTrue();
        catalog.DerivedTypes.ShouldContain(typeof(GeneratedCallMarkerImplementation));
    }

    [Fact]
    public void TypeOfCall_UsesTheGeneratedCatalog() {
        RuntimeTypeCache.Clear();

        var types = RuntimeTypeCache.TypesDerivedFrom(typeof(IGeneratedCallMarker));

        types.ShouldContain(typeof(GeneratedCallMarkerImplementation));
    }

    [Fact]
    public void BindGenericCall_UsesTheGeneratedCatalogForTheInitialSnapshot() {
        RuntimeTypeCache.Clear();
        IReadOnlyList<Type>? received = null;

        using var binding = RuntimeTypeCache.Bind<IGeneratedCallMarker>(types => received = types);

        received.ShouldNotBeNull();
        received.ShouldContain(typeof(GeneratedCallMarkerImplementation));
    }

    [Fact]
    public void QueryMethods_CarryValidGeneratorTargetMetadata() {
        var methods = typeof(RuntimeTypeCache)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(method => method.Name is nameof(RuntimeTypeCache.TypesDerivedFrom) or nameof(RuntimeTypeCache.Bind))
            .ToArray();

        var invalidMethods = methods
            .Where(method => !HasValidTarget(method))
            .Select(method => method.ToString())
            .ToArray();

        invalidMethods.ShouldBeEmpty();
    }

    static RuntimeTypeCacheGeneratedTypesAttribute GetCatalog(Type baseType) =>
        typeof(RuntimeTypeCacheSourceGenerationTests).Assembly
            .GetCustomAttributes<RuntimeTypeCacheGeneratedTypesAttribute>()
            .Single(attribute => attribute.BaseType == baseType);

    static bool HasValidTarget(MethodInfo method) {
        var targets = method.GetCustomAttributes<RuntimeTypeCacheSourceGenerationTargetAttribute>().ToArray();
        if (targets.Length != 1) {
            return false;
        }

        var target = targets[0];
        return target.Source switch {
            RuntimeTypeCacheQuerySource.GenericTypeArgument =>
                method.IsGenericMethodDefinition && target.Index < method.GetGenericArguments().Length,
            RuntimeTypeCacheQuerySource.MethodArgument =>
                target.Index < method.GetParameters().Length &&
                method.GetParameters()[target.Index].ParameterType == typeof(Type),
            _ => false,
        };
    }
}
