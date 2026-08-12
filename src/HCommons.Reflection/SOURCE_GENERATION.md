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
  only when the corresponding argument is a direct `typeof(...)` operation. Its operand can be a
  wrapper type parameter that becomes concrete through source-visible calls.

The generator discovers methods through this metadata rather than method names. When adding
or changing a query API, update its marker, XML documentation, generator tests, architecture
test, consumer README examples, and benchmarks together.

Predicate overloads still establish only a base-type query. Catalogs must contain the complete
assignable set; runtime filtering is deliberately applied after generated or reflected discovery.
Never specialize catalog contents based on a predicate expression.

The generator records open query templates inside source-declared wrapper methods and resolves them
from wrapper invocation sites in the same compilation. Resolution follows transitive method calls,
substitutes method and containing-type parameters, and supports constructed query types. A method
definition is visited at most once along a traversal so recursive generic call graphs cannot expand
without bound. Wrapper bodies available only as referenced metadata, delegate or method-group
dispatch, and templates that never resolve to a closed named type retain reflection fallback.

This includes `RuntimeTypeFilter` descriptor overloads. Every new query overload must retain the
source-generation target marker even when its filter could theoretically be interpreted at compile
time. Catalog completeness describes assignability, not a particular runtime filter expression.

## Catalog completeness

The generator emits `RuntimeTypeCacheGeneratedTypesAttribute` at assembly level for each
concrete query it discovers from a call, `[GenerateRuntimeTypeCache]`, or referenced catalog.
Catalogs contain only types declared by that compilation.

- A complete catalog lets the runtime skip `Assembly.GetTypes()` for that assembly/base pair.
- An empty complete catalog is meaningful: the assembly contains no matches.
- Private nested and file-local types cannot be referenced by assembly-level generated code.
  Their catalog is marked incomplete and the runtime scans that assembly instead.
- Generic type parameters and open generic queries that cannot be closed through source-visible
  wrapper invocations, along with runtime `Type` variables, are not compile-time queries and retain
  reflection behavior.
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

## RuntimeTypeFilter contract

`RuntimeTypeFilter` is a non-generic readonly struct. Built-in AND-only conditions use flags and
must remain allocation-free. Rule and Boolean expression nodes are immutable records. Composition
is left-associative, preserves short-circuit evaluation order, and uses structural rather than full
Boolean-equivalence equality.

- `Concrete`, `Public`, `Closed`, and the constructor condition are built-in flags.
- `Instantiable` is the canonical combination of concrete, closed, and public-parameterless-
  constructor flags. It deliberately does not imply external type visibility.
- Chained built-ins mean AND. `And` combines a grouped filter, `Or` combines the complete accumulated
  expression, instance `Not(other)` means AND NOT, and static `RuntimeTypeFilters.Not(filter)`
  negates the supplied expression.
- `Where(Func<Type, bool>)` is always uncacheable.
- `Where<TState>(TState, Func<TState, Type, bool>)` is also uncacheable. It exists to avoid closure
  allocation: value-type state is embedded in the generic expression node without boxing, and a
  static lambda allows the compiler to reuse the delegate. The expression node still allocates.
- `Where(RuntimeTypeFilterRule)` is cacheable. Derived records must be immutable, behaviorally pure,
  and include every matching input in record equality.
- `Cached()` is an explicit request for both queries and bindings. Equal uncached descriptors may
  read an already registered entry but never create one.
- The cache key excludes the cache-request bit. Filtered snapshots are stored per `QueryEntry` and
  invalidated on `Clear()` or when their source snapshot changes.
- User predicates and rules must be evaluated outside `RuntimeTypeCache.s_gate`. Equality and hash
  implementations used as cache keys must remain pure and non-blocking.

The bundled analyzer reports `HCRTCFILTER001` when it can see `Cached()` and either delegate-based
`Where` overload in the same fluent expression. The runtime must still ignore caching for every
uncacheable descriptor because analyzers cannot prove values that flow through variables. Keep the
analyzer, runtime `IsCacheable` behavior, package README, diagnostic tests, and Unity analyzer
packaging in sync whenever custom filtering changes.

Stateful predicate overloads intentionally live on `RuntimeTypeFilter` and `RuntimeTypeFilters`,
not directly on every `RuntimeTypeCache` query and binding method. Callers can construct a
descriptor inline without adding a state type parameter to the already-large cache overload
surface or running into partial generic type-inference limitations.

Potential parameterized built-ins such as namespace, assembly, or attribute filters should be
implemented as immutable `RuntimeTypeFilterRule` records rather than expanding the flag set.

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
