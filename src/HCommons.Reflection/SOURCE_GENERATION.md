# RuntimeTypeCache source-generation contract

`HCommons.Reflection.SourceGeneration` is built and shipped inside the `HCommons.Reflection`
NuGet package. Its output is an optimization and preservation aid; reflection remains the
correctness fallback whenever generated coverage is absent or incomplete.

## Runtime entry points

Every public `RuntimeTypeCache` method that establishes a base-type query must carry one
`RuntimeTypeCacheSourceGenerationTargetAttribute`:

- Use `GenericTypeArgument` and the generic argument index for APIs such as
  `TypesDerivedFrom<TBase>()` and `Bind<TBase>(...)`.
- Use `MethodArgument` and the parameter index for APIs accepting `Type`. Generation occurs
  only when the corresponding argument is a direct `typeof(...)` operation.

The generator discovers methods through this metadata rather than method names. When adding
or changing a query API, update its marker, XML documentation, generator tests, architecture
test, consumer README examples, and benchmarks together.

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
