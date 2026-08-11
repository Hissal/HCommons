# HCommons.Reflection

`HCommons.Reflection` discovers types assignable to a class or interface across the assemblies
loaded in the current application domain. Results are cached, late-loaded assemblies are merged
incrementally, and an included source generator can replace reflection with compile-time type
catalogs on an assembly-by-assembly basis.

The library has no dependency on Unity and can be used in ordinary .NET applications, Unity
players, editor tooling, plug-in systems, dependency injection, and other type-registration code.

## Installation

```bash
dotnet add package HCommons.Reflection
```

The package contains both the runtime library and its source generator. The generator is packed
at `analyzers/dotnet/cs/HCommons.Reflection.SourceGeneration.dll` and is enabled automatically by
normal `PackageReference` builds.

## Discovering types

Use the generic API when the base type is known at compile time:

```csharp
using HCommons.Reflection;

IReadOnlyList<Type> handlers = RuntimeTypeCache.TypesDerivedFrom<IHandler>();
```

Use the `Type` overload for runtime-selected queries:

```csharp
Type contract = SelectContract();
IReadOnlyList<Type> implementations = RuntimeTypeCache.TypesDerivedFrom(contract);
```

Each result is an immutable snapshot containing every currently loaded type for which
`baseType.IsAssignableFrom(type)` is true, except the queried base type itself. This includes
indirect implementations, interfaces, abstract classes, value types, and open generic type
definitions when they satisfy normal runtime assignability rules. Result order is unspecified.

Snapshots are safe to retain and enumerate while other assemblies load. A later query returns a
new snapshot if new matches have appeared; an existing snapshot is never mutated.

## Filtering results

Pass a `Func<Type, bool>` when only part of the assignable type set is relevant:

```csharp
IReadOnlyList<Type> concreteHandlers = RuntimeTypeCache.TypesDerivedFrom<IHandler>(
    type => !type.IsAbstract && !type.IsInterface);
```

The runtime-selected form accepts the same predicate:

```csharp
IReadOnlyList<Type> concreteHandlers = RuntimeTypeCache.TypesDerivedFrom(
    typeof(IHandler),
    type => !type.IsAbstract && !type.IsInterface);
```

The predicate receives only types already assignable to the requested base type; the base type
itself has already been excluded. The returned filtered snapshot is immutable and its order is
unspecified.

Filtering happens after the shared base-type query is resolved. Generated catalogs and reflected
assemblies therefore use identical filter behavior, and separate filters reuse the same unfiltered
cache. A `TypesDerivedFrom` predicate is evaluated on every call rather than cached because a
delegate can capture mutable state. Predicate exceptions propagate to the caller.

Filtered bindings use the same argument order: base type, filter, then callback:

```csharp
using IDisposable subscription = RuntimeTypeCache.Bind<IHandler>(
    type => !type.IsAbstract && !type.IsInterface,
    handlers => RebuildHandlerRegistry(handlers));
```

They deliver an initial filtered snapshot synchronously and notify again only when the filtered
type set changes. A newly loaded assignable type that fails the predicate does not trigger the user
callback. Keep a binding predicate behaviorally stable for the lifetime of its subscription;
dispose and recreate the binding when its criteria change. Predicates run on the same thread or
synchronization context as the corresponding callback. As with callback exceptions, binding
predicate exceptions other than `OutOfMemoryException` are written as trace warnings.

## Observing late-loaded assemblies

`Bind` delivers an initial snapshot synchronously and replacement snapshots when newly loaded
assemblies add matching types:

```csharp
using IDisposable subscription = RuntimeTypeCache.Bind<IHandler>(handlers => {
    RebuildHandlerRegistry(handlers);
});
```

The overload without a synchronization-context argument captures
`SynchronizationContext.Current` when the binding is created. Supply a context explicitly when
callbacks must run on a particular thread:

```csharp
SynchronizationContext uiContext = SynchronizationContext.Current!;

using IDisposable subscription = RuntimeTypeCache.Bind<IHandler>(
    handlers => RebuildUi(handlers),
    uiContext);
```

Pass `null` to dispatch later notifications through the thread pool:

```csharp
using IDisposable subscription = RuntimeTypeCache.Bind<IHandler>(OnHandlersChanged, null);
```

Dispose the returned subscription to stop notifications. Rapid changes can be coalesced into the
latest snapshot. Callback exceptions other than `OutOfMemoryException` are written as trace
warnings and do not stop the cache worker.

The equivalent runtime-type overloads are also available:

```csharp
using IDisposable subscription = RuntimeTypeCache.Bind(
    typeof(IHandler),
    OnHandlersChanged);
```

## Clearing the cache

Call `RuntimeTypeCache.Clear()` when cached assembly contents must be rebuilt:

```csharp
RuntimeTypeCache.Clear();
IReadOnlyList<Type> handlers = RuntimeTypeCache.TypesDerivedFrom<IHandler>();
```

Without active bindings, rebuilding is deferred until the next query or binding. With active
bindings, a background rebuild is scheduled and a replacement snapshot is delivered only when
the result changed. Bindings remain registered across `Clear()`.

Assembly-load notifications detect newly loaded assemblies, but they cannot detect types added to
an already loaded dynamic assembly. Reflection-backed queries can use `Clear()` to rescan such an
assembly. A complete generated catalog is fixed at compilation time and intentionally remains the
authority for its assembly/base-type pair.

The cache and returned snapshots hold strong references to `Assembly` and `Type` instances. Clear
the cache, dispose bindings, and release snapshots before unloading a collectible
`AssemblyLoadContext`.

## Source generation

Source generation is a transparent optimization. Runtime behavior remains correct through
reflection whenever a generated catalog is absent or incomplete.

### Automatically discovered queries

The generator recognizes concrete generic calls:

```csharp
RuntimeTypeCache.TypesDerivedFrom<IHandler>();
RuntimeTypeCache.TypesDerivedFrom<IHandler>(type => !type.IsAbstract);
RuntimeTypeCache.Bind<IHandler>(OnHandlersChanged);
RuntimeTypeCache.Bind<IHandler>(type => !type.IsAbstract, OnHandlersChanged);
```

It also recognizes a direct `typeof(...)` argument:

```csharp
RuntimeTypeCache.TypesDerivedFrom(typeof(IHandler));
RuntimeTypeCache.TypesDerivedFrom(typeof(IHandler), type => !type.IsAbstract);
RuntimeTypeCache.Bind(typeof(IHandler), OnHandlersChanged);
RuntimeTypeCache.Bind(typeof(IHandler), type => !type.IsAbstract, OnHandlersChanged);
```

Aliases and fully qualified method calls are supported because discovery uses Roslyn symbols, not
method-name text.

Predicates do not change catalog completeness. The generator records every assignable type for the
base query, and the runtime applies the predicate to that complete snapshot. The predicate itself
is not analyzed or executed at compile time.

A runtime `Type` variable is not a compile-time query and therefore uses reflection:

```csharp
Type contract = SelectContract();
RuntimeTypeCache.TypesDerivedFrom(contract);
```

Open generic base queries and base types containing type parameters are also left to reflection.

### Queries shared across assemblies

Apply `[GenerateRuntimeTypeCache]` to a shared base contract when implementations live in other
assemblies:

```csharp
using HCommons.Reflection;

[GenerateRuntimeTypeCache]
public interface IHandler { }
```

The contract assembly publishes the query in its metadata. Every downstream assembly compiled
with the HCommons generator sees that metadata and emits its own catalog of locally declared
implementations.

For reliable cross-assembly coverage:

1. Reference `HCommons.Reflection` directly from the contract project and every project that
   declares implementations. Do not assume analyzer assets flow through an unrelated package or
   project reference.
2. Build the contract assembly before its implementation assemblies. Normal project references
   establish this order automatically.
3. Keep matching types accessible from generated assembly-level code. Top-level `public` and
   `internal` types work. Private nested and file-local matching types make the catalog incomplete.
4. Ensure implementations are visible as C# source during the compilation. Types introduced later
   by IL weaving or a peer source generator cannot be included in this generator's catalog.

One inaccessible matching type makes that assembly/base-type catalog incomplete and produces the
informational diagnostic `HCRTCGEN001`. The runtime then reflects over the assembly so that no
discoverable types are lost.

### What a generated catalog looks like

For this source:

```csharp
[GenerateRuntimeTypeCache]
public interface IHandler { }

public sealed class FileHandler : IHandler { }
internal sealed class NetworkHandler : IHandler { }
```

the generator emits metadata equivalent to:

```csharp
[assembly: RuntimeTypeCacheGeneratedTypes(
    typeof(IHandler),
    true,
    typeof(FileHandler),
    typeof(NetworkHandler))]
```

Catalog completeness is evaluated separately for every assembly and exact base type. A complete
catalog allows `RuntimeTypeCache` to skip `Assembly.GetTypes()` only for that pair. Other queries
or assemblies without complete catalogs retain reflection fallback.

An empty complete catalog is meaningful:

```csharp
[assembly: RuntimeTypeCacheGeneratedTypes(typeof(IHandler), true)]
```

It states that the declaring assembly contains no types assignable to `IHandler`, allowing the
runtime to skip scanning that assembly for this query.

### Manually declaring a catalog

`RuntimeTypeCacheGeneratedTypesAttribute` is public so generated build tooling can communicate
with the runtime, but application code normally should not use it directly. The bundled source
generator is safer because an incorrect complete catalog causes real implementations to be
omitted from results.

If another build-time tool already knows the complete type set, place the attribute at assembly
level in any source file belonging to the assembly being described:

```csharp
using HCommons.Reflection;

[assembly: RuntimeTypeCacheGeneratedTypes(
    typeof(IHandler),
    true,
    typeof(FileHandler),
    typeof(NetworkHandler))]
```

The rules are strict:

- The attribute describes only types declared by its own assembly; do not list implementations
  from another assembly.
- List every declared type assignable to the exact base type, excluding the base type itself.
- `isComplete: true` promises that the list is exhaustive. An empty list promises there are no
  matches in the assembly.
- Multiple attributes for the same base type are combined, which permits chunking large lists.
  Every chunk must have `isComplete: true`; one incomplete chunk forces reflection.
- Every listed type must be non-null, different from the base type, and assignable to it. A malformed
  entry makes the combined catalog incomplete and forces reflection.
- `isComplete: false` deliberately requests reflection. Listed types are not used as a partial
  optimization because the full assembly must still be scanned for correctness.

For example, a manually chunked complete catalog is valid:

```csharp
[assembly: RuntimeTypeCacheGeneratedTypes(
    typeof(IHandler), true, typeof(FileHandler))]

[assembly: RuntimeTypeCacheGeneratedTypes(
    typeof(IHandler), true, typeof(NetworkHandler))]
```

Do not manually add this attribute merely to enable generation. Use
`[GenerateRuntimeTypeCache]` on the base contract for that purpose.

### Verifying generator coverage

Reflection fallback makes missing generator configuration easy to overlook. A test in each
implementation assembly can enforce complete coverage:

```csharp
using System.Reflection;
using HCommons.Reflection;

Assembly assembly = typeof(FileHandler).Assembly;

RuntimeTypeCacheGeneratedTypesAttribute[] catalogs = assembly
    .GetCustomAttributes<RuntimeTypeCacheGeneratedTypesAttribute>()
    .Where(catalog => catalog.BaseType == typeof(IHandler))
    .ToArray();

Assert.NotEmpty(catalogs);
Assert.All(catalogs, catalog => Assert.True(catalog.IsComplete));
```

Large catalogs are split across multiple attributes, so verify all matching entries rather than
expecting exactly one.

## Unity

The generator targets .NET Standard 2.0 and Microsoft.CodeAnalysis.CSharp 4.3, matching the Unity
6 source-generator toolchain. It does not use module initializers.

When installing through NuGetForUnity, verify that
`HCommons.Reflection.SourceGeneration.dll` is imported as an analyzer:

- All plug-in platforms are disabled.
- The exact, case-sensitive `RoslynAnalyzer` asset label is assigned.
- The analyzer's Unity folder/assembly-definition scope includes every `.asmdef` that declares
  implementations.

Older NuGetForUnity versions can apply analyzer import settings after Unity's first asset refresh.
If Unity initially tries to load `Microsoft.CodeAnalysis` as a player/runtime dependency, update
NuGetForUnity or correct the generator DLL's importer settings and reimport it.

Unity managed-code stripping can remove types that are discovered only through reflection. Use
`link.xml`, `[UnityEngine.Scripting.Preserve]`, or another linker-preservation mechanism for
reflection-backed types. Generated catalog entries create static type references, but preservation
requirements should still be validated for each IL2CPP/linker configuration.

## Trimming and Native AOT

The discovery APIs carry `RequiresUnreferencedCode` on modern .NET targets because reflection
fallback cannot guarantee that matching types survive trimming. Complete generated catalogs reduce
reflection, but uncovered assemblies and runtime-selected queries still require preservation
configuration. Treat trimming warnings as actionable unless every relevant assembly/base-type pair
is covered and the resulting publication has been verified.

## Performance characteristics

Cached queries return an immutable snapshot without rescanning assemblies. Generation primarily
optimizes cold queries, cache rebuilds, and late assembly processing by avoiding
`Assembly.GetTypes()` for covered assembly/base-type pairs.

The aggregate improvement depends on coverage. A process containing many uncovered framework,
third-party, or plug-in assemblies can still spend most of a cold query in reflection even when
the application's own catalog is complete. Measure the intended application or Unity player
assembly layout rather than assuming a whole-process speedup.

## Thread safety

Public cache operations are synchronized and can be called from multiple threads. Returned
snapshots are immutable. `Bind` callback threading is controlled by the captured or supplied
`SynchronizationContext`; callbacks should still avoid blocking for long periods.

## Target frameworks

- .NET 9.0
- .NET 8.0
- .NET Standard 2.1
- .NET Standard 2.0

## License

HCommons.Reflection is distributed under the MIT license.
