# RuntimeTypeCache source-generation contract

`HCommons.Reflection.SourceGeneration` is built and shipped inside the `HCommons.Reflection`
NuGet package. Its output is an optimization and preservation aid; reflection remains the
correctness fallback whenever generated coverage is absent or incomplete.

## Runtime entry points

Every public `RuntimeTypeCache` method that establishes a base-type query must carry one
`RuntimeTypeCacheSourceGenerationTargetAttribute`:

- Use `GenericTypeArgument` and the generic argument index for APIs such as
  `TypesDerivedFrom<TBase>()`, their filtered overloads, and `Bind<TBase>(...)`.
- Use `MethodArgument` and the parameter index for APIs accepting `Type`. Generation occurs
  only when the corresponding argument is a direct `typeof(...)` operation.

The generator discovers methods through this metadata rather than method names. When adding
or changing a query API, update its marker, XML documentation, generator tests, architecture
test, consumer README examples, and benchmarks together.

Predicate overloads still establish only a base-type query. Catalogs must contain the complete
assignable set; runtime filtering is deliberately applied after generated or reflected discovery.
Never specialize catalog contents based on a predicate expression.

## Catalog completeness

The generator emits `RuntimeTypeCacheGeneratedTypesAttribute` at assembly level for each
concrete query it discovers from a call, `[GenerateRuntimeTypeCache]`, or referenced catalog.
Catalogs contain only types declared by that compilation.

- A complete catalog lets the runtime skip `Assembly.GetTypes()` for that assembly/base pair.
- An empty complete catalog is meaningful: the assembly contains no matches.
- Private nested and file-local types cannot be referenced by assembly-level generated code.
  Their catalog is marked incomplete and the runtime scans that assembly instead.
- Generic type parameters, open generic queries, and runtime `Type` variables are not
  compile-time queries and retain reflection behavior.
- Result order remains unspecified, the queried base is excluded, and indirect classes,
  interfaces, abstract types, value types, and delegates must follow `Type.IsAssignableFrom`
  semantics.

Never mark a catalog complete unless it describes every matching type in its declaring
assembly. Runtime validation treats malformed generated entries as incomplete.

## Cross-assembly behavior

Use `[GenerateRuntimeTypeCache]` on shared base contracts when implementation assemblies are
compiled independently. The base assembly emits query metadata, and downstream compilations
propagate it while cataloging their own declared implementations. Assemblies built without the
generator continue to work through reflection.

Late-loaded assemblies are processed through the existing `AssemblyLoad` path. Generated and
reflected matches use the same immutable snapshots and `Bind` notification machinery.

## Performance expectations

Coverage and reflection fallback are decided independently for every assembly/base-type pair.
Generation removes `Assembly.GetTypes()` and its temporary type array for covered pairs, but a
cold query still scans every loaded assembly without a complete catalog. Consequently, the gain
depends on how much of the loaded type metadata belongs to assemblies built with the generator.
Cached queries retain the existing constant-time snapshot return and should not materially change.
Filtered queries additionally evaluate their predicate over the cached base snapshot. Arbitrary
delegates are not cache keys because their captured state can change without changing delegate
identity.

## Possible built-in filters

No built-in filters are currently part of the public API. If common usage justifies them, the most
natural first surface is a `RuntimeTypeFilters` class exposing reusable `Func<Type, bool>` values
and `All`, `Any`, and `Not` combinators. Useful candidates include:

- `Concrete`: excludes interfaces and abstract types.
- `Closed`: excludes types whose `ContainsGenericParameters` is true.
- `Instantiable`: combines concrete and closed checks, with an explicit decision about value types
  and constructor requirements.
- `Public`: accepts top-level public and nested public types.
- `HasPublicParameterlessConstructor`: useful for simple activator and registration scenarios.
- `HasAttribute<TAttribute>(inherit)`: a parameterized predicate for metadata-driven discovery.
- Assembly or namespace predicates for plug-in boundaries.

Keep `Concrete` distinct from `Instantiable`: a non-abstract open generic type is concrete but
cannot be directly constructed. Prefer predicates and combinators over an enum flags API because
attributes, assemblies, namespaces, and constructor rules need parameters and compose more clearly
as functions.

If profiling later shows repeated built-in filtering to be significant, a separate immutable
filter descriptor with stable equality could allow filtered snapshots to be cached by
`(baseType, filterDescriptor)`. Preserve the `Func<Type, bool>` overload as the flexible uncached
path; do not infer cacheability from delegate identity.

The mixed-coverage benchmark is deliberately retained: it compares `ReflectionFullRebuild` and
`GeneratedFullRebuild` after `RuntimeTypeCache.Clear()`. Do not advertise an aggregate speedup
unless that benchmark shows one in the intended application/Unity assembly layout.

## Compiler and package compatibility

The generator targets .NET Standard 2.0, references Microsoft.CodeAnalysis.CSharp 4.3, and
emits C# 9-compatible source. Avoid module initializers because Unity 6 does not support them.
The DLL is packed at `analyzers/dotnet/cs`, which is consumed by normal PackageReference builds
and recognized by NuGetForUnity's Roslyn-analyzer handling. Unity requires analyzer DLLs to have
all plugin platforms disabled and the exact `RoslynAnalyzer` asset label. Older NuGetForUnity
versions can apply those settings only after the first asset refresh, so verify the DLL importer
or update NuGetForUnity if Unity initially tries to load the generator as a runtime plug-in.
