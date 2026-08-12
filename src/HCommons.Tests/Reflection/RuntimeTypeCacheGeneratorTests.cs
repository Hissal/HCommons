using System.Collections.Immutable;
using System.Reflection;
using HCommons.Reflection;
using HCommons.Reflection.SourceGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HCommons.Tests;

public sealed class RuntimeTypeCacheGeneratorTests {
    [Fact]
    public void Generator_DiscoversGenericAndTypeOfCallsThroughMethodMetadata() {
        const string source = """
            using System;
            using System.Collections.Generic;
            using HCommons.Reflection;
            using Cache = HCommons.Reflection.RuntimeTypeCache;

            public interface IMarker { }
            public abstract class MarkerBase : IMarker { }
            internal sealed class Marker : MarkerBase { }

            public static class Queries {
                public static void Run() {
                    _ = Cache.TypesDerivedFrom<IMarker>();
                    _ = Cache.TypesDerivedFrom(typeof(IMarker));
                    _ = Cache.TypesDerivedFrom<IMarker>(type => !type.IsAbstract);
                    _ = Cache.TypesDerivedFrom(typeof(IMarker), type => !type.IsAbstract);
                    _ = Cache.TypesDerivedFrom<IMarker>(RuntimeTypeFilters.Concrete());
                    _ = Cache.TypesDerivedFrom(typeof(IMarker), RuntimeTypeFilters.Concrete());
                    _ = Cache.Bind<IMarker>(_ => { });
                    _ = Cache.Bind(typeof(IMarker), _ => { });
                    _ = Cache.Bind<IMarker>(type => !type.IsAbstract, _ => { });
                    _ = Cache.Bind(typeof(IMarker), type => !type.IsAbstract, _ => { });
                    _ = Cache.Bind<IMarker>(RuntimeTypeFilters.Concrete(), _ => { });
                    _ = Cache.Bind(typeof(IMarker), RuntimeTypeFilters.Concrete(), _ => { });
                }
            }
            """;

        var result = RunGenerator(source);

        result.GeneratedSource!.ShouldContain("typeof(global::IMarker), true");
        result.GeneratedSource!.ShouldContain("typeof(global::MarkerBase)");
        result.GeneratedSource!.ShouldContain("typeof(global::Marker)");
    }

    [Fact]
    public void Generator_DiscoversQueriesThroughGenericWrapperCalls() {
        const string source = """
            using HCommons.Reflection;

            public interface IFirstMarker { }
            public interface ISecondMarker { }
            public sealed class FirstMarker : IFirstMarker { }
            public sealed class SecondMarker : ISecondMarker { }

            public static class Queries {
                public static void Run() {
                    RegisterAll<IFirstMarker>();
                    RegisterAll<ISecondMarker>();
                }

                private static void RegisterAll<TContract>() {
                    _ = RuntimeTypeCache.TypesDerivedFrom<TContract>(
                        RuntimeTypeFilters.Instantiable().Cached());
                }
            }
            """;

        var result = RunGenerator(source);

        var generatedSource = result.GeneratedSource!;
        generatedSource.ShouldContain("typeof(global::IFirstMarker), true");
        generatedSource.ShouldContain("typeof(global::FirstMarker)");
        generatedSource.ShouldContain("typeof(global::ISecondMarker), true");
        generatedSource.ShouldContain("typeof(global::SecondMarker)");
    }

    [Fact]
    public void Generator_DiscoversQueriesThroughTransitiveGenericWrappers() {
        const string source = """
            using HCommons.Reflection;

            public interface IMarker { }
            public sealed class Marker : IMarker { }

            public static class Queries {
                public static void Run() => First<IMarker>();

                private static void First<T>() => Second<T>();

                private static void Second<T>() => Third<T>();

                private static void Third<T>() {
                    _ = RuntimeTypeCache.TypesDerivedFrom<T>();
                }
            }
            """;

        var result = RunGenerator(source);

        var generatedSource = result.GeneratedSource!;
        generatedSource.ShouldContain("typeof(global::IMarker), true");
        generatedSource.ShouldContain("typeof(global::Marker)");
    }

    [Fact]
    public void Generator_SubstitutesConstructedQueriesAndContainingTypeParameters() {
        const string source = """
            using HCommons.Reflection;

            public interface IHandler<T> { }
            public sealed class IntHandler : IHandler<int> { }

            public sealed class Registrar<T> {
                public void Run() {
                    _ = RuntimeTypeCache.TypesDerivedFrom<IHandler<T>>();
                }
            }

            public static class Queries {
                public static void Run() => new Registrar<int>().Run();
            }
            """;

        var result = RunGenerator(source);

        var generatedSource = result.GeneratedSource!;
        generatedSource.ShouldContain("typeof(global::IHandler<int>), true");
        generatedSource.ShouldContain("typeof(global::IntHandler)");
    }

    [Fact]
    public void Generator_DiscoversTypeOfQueriesThroughGenericWrappers() {
        const string source = """
            using HCommons.Reflection;

            public interface IMarker { }
            public sealed class Marker : IMarker { }

            public sealed class Contract<T> { }

            public static class Queries {
                public static void Run() => Register(new Contract<IMarker>());

                private static void Register<T>(Contract<T> contract) {
                    _ = RuntimeTypeCache.TypesDerivedFrom(typeof(T));
                }
            }
            """;

        var result = RunGenerator(source);

        var generatedSource = result.GeneratedSource!;
        generatedSource.ShouldContain("typeof(global::IMarker), true");
        generatedSource.ShouldContain("typeof(global::Marker)");
    }

    [Fact]
    public void Generator_RecursiveWrapperChainTerminatesAndDeduplicatesQueries() {
        const string source = """
            using HCommons.Reflection;

            public interface IMarker { }
            public sealed class Marker : IMarker { }

            public static class Queries {
                public static void Run() => First<IMarker>();

                private static void First<T>() {
                    _ = RuntimeTypeCache.TypesDerivedFrom<T>();
                    Second<T>();
                }

                private static void Second<T>() => First<T>();
            }
            """;

        var result = RunGenerator(source);

        const string catalog = "typeof(global::IMarker), true";
        result.GeneratedSource!.IndexOf(catalog, StringComparison.Ordinal).ShouldBeGreaterThanOrEqualTo(0);
        result.GeneratedSource.IndexOf(catalog, StringComparison.Ordinal).ShouldBe(
            result.GeneratedSource.LastIndexOf(catalog, StringComparison.Ordinal));
    }

    [Fact]
    public void Generator_UninstantiatedGenericWrapperDoesNotCreateACompileTimeQuery() {
        const string source = """
            using HCommons.Reflection;

            public interface IMarker { }
            public sealed class Marker : IMarker { }

            public static class Queries {
                private static void Register<T>() {
                    _ = RuntimeTypeCache.TypesDerivedFrom<T>();
                }
            }
            """;

        var result = RunGenerator(source);

        result.GeneratedSource.ShouldBeNull();
    }

    [Fact]
    public void Generator_ExternalGenericWrapperDoesNotCreateACompileTimeQuery() {
        const string wrapperSource = """
            using HCommons.Reflection;

            public static class ExternalQueries {
                public static void Register<T>() {
                    _ = RuntimeTypeCache.TypesDerivedFrom<T>();
                }
            }
            """;
        const string consumerSource = """
            public interface IMarker { }
            public sealed class Marker : IMarker { }

            public static class Queries {
                public static void Run() => ExternalQueries.Register<IMarker>();
            }
            """;
        var wrapper = CreateCompilation(wrapperSource, "ExternalWrapper");
        using var wrapperAssembly = new MemoryStream();
        var emitResult = wrapper.Emit(
            wrapperAssembly,
            cancellationToken: TestContext.Current.CancellationToken);
        emitResult.Success.ShouldBeTrue(string.Join(Environment.NewLine, emitResult.Diagnostics));
        wrapperAssembly.Position = 0;
        var wrapperReference = MetadataReference.CreateFromStream(wrapperAssembly);
        var consumer = CreateCompilation(
            consumerSource,
            "ExternalWrapperConsumer",
            wrapperReference);

        var result = RunGenerator(consumer);

        result.GeneratedSource.ShouldBeNull();
    }

    [Fact]
    public void Generator_TypeVariableDoesNotCreateACompileTimeQuery() {
        const string source = """
            using System;
            using HCommons.Reflection;

            public interface IMarker { }
            public sealed class Marker : IMarker { }

            public static class Queries {
                public static void Run() {
                    Type markerType = typeof(IMarker);
                    _ = RuntimeTypeCache.TypesDerivedFrom(markerType);
                }
            }
            """;

        var result = RunGenerator(source);

        result.GeneratedSource.ShouldBeNull();
    }

    [Fact]
    public void Generator_ExplicitAttributeCreatesAnEmptyCompleteCatalog() {
        const string source = """
            using HCommons.Reflection;

            [GenerateRuntimeTypeCache]
            public interface IMarker { }
            """;

        var result = RunGenerator(source);

        result.GeneratedSource!.ShouldContain("typeof(global::IMarker), true");
    }

    [Fact]
    public void Generator_InaccessibleMatchCreatesAnIncompleteCatalogDiagnostic() {
        const string source = """
            using HCommons.Reflection;

            [GenerateRuntimeTypeCache]
            public interface IMarker { }

            public static class Container {
                private sealed class Marker : IMarker { }
            }
            """;

        var result = RunGenerator(source);

        result.GeneratedSource!.ShouldContain("typeof(global::IMarker), false");
        result.Diagnostics.ShouldContain(diagnostic => diagnostic.Id == "HCRTCGEN001");
    }

    [Fact]
    public void Generator_ReferencedCatalogPropagatesTheQueryToADownstreamCompilation() {
        const string contractSource = """
            using HCommons.Reflection;

            [GenerateRuntimeTypeCache]
            public interface IMarker { }
            """;
        const string implementationSource = """
            public sealed class Marker : IMarker { }
            """;
        var contract = CreateCompilation(contractSource, "GeneratedContract");
        var contractGeneration = RunGenerator(contract);
        using var contractAssembly = new MemoryStream();
        var emitResult = contractGeneration.OutputCompilation.Emit(
            contractAssembly,
            cancellationToken: TestContext.Current.CancellationToken);
        emitResult.Success.ShouldBeTrue(string.Join(Environment.NewLine, emitResult.Diagnostics));
        contractAssembly.Position = 0;
        var contractReference = MetadataReference.CreateFromStream(contractAssembly);
        var implementation = CreateCompilation(
            implementationSource,
            "GeneratedImplementation",
            contractReference);

        var result = RunGenerator(implementation);

        result.GeneratedSource!.ShouldContain("typeof(global::IMarker), true");
        result.GeneratedSource!.ShouldContain("typeof(global::Marker)");
    }

    [Fact]
    public async Task FilterAnalyzer_WarnsOnlyWhenDelegateWhereRequestsCaching() {
        const string source = """
            using System;
            using HCommons.Reflection;

            public sealed record ExactTypeRule(Type Expected) : RuntimeTypeFilterRule {
                public override bool Matches(Type type) => type == Expected;
            }

            public readonly struct FilterState {
                public bool Matches(Type type) => type.IsClass;
            }

            public static class Filters {
                public static void Build() {
                    _ = RuntimeTypeFilters.Where(type => type.IsClass).Cached();
                    _ = RuntimeTypeFilters.Concrete().Cached().Where(type => type.IsClass);
                    _ = RuntimeTypeFilters.Where(
                        new FilterState(),
                        static (state, type) => state.Matches(type)).Cached();
                    _ = RuntimeTypeFilters.Concrete().Cached().Where(
                        new FilterState(),
                        static (state, type) => state.Matches(type));
                    _ = RuntimeTypeFilters.Where(new ExactTypeRule(typeof(string))).Cached();
                }
            }
            """;
        var compilation = CreateCompilation(source, "FilterAnalyzerTests");
        var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(new RuntimeTypeFilterAnalyzer());

        var diagnostics = await compilation
            .WithAnalyzers(analyzers, cancellationToken: TestContext.Current.CancellationToken)
            .GetAnalyzerDiagnosticsAsync(TestContext.Current.CancellationToken);

        diagnostics.Count(diagnostic => diagnostic.Id == "HCRTCFILTER001").ShouldBe(4);
    }

    static GeneratorResult RunGenerator(string source) =>
        RunGenerator(CreateCompilation(source, "GeneratorTests"));

    static GeneratorResult RunGenerator(CSharpCompilation compilation) {
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            new[] { new RuntimeTypeCacheGenerator().AsSourceGenerator() },
            parseOptions: (CSharpParseOptions)compilation.SyntaxTrees.First().Options);
        driver = driver.RunGeneratorsAndUpdateCompilation(
            compilation,
            out var outputCompilation,
            out var generatorDiagnostics);
        var runResult = driver.GetRunResult();
        var generatedSource = runResult.GeneratedTrees.Length == 0
            ? null
            : string.Join(Environment.NewLine, runResult.GeneratedTrees.Select(tree => tree.ToString()));
        var diagnostics = generatorDiagnostics
            .Concat(runResult.Diagnostics)
            .Distinct()
            .ToImmutableArray();

        return new GeneratorResult((CSharpCompilation)outputCompilation, generatedSource, diagnostics);
    }

    static CSharpCompilation CreateCompilation(
        string source,
        string assemblyName,
        params MetadataReference[] additionalReferences) {
        var references = GetFrameworkReferences()
            .Append(MetadataReference.CreateFromFile(typeof(RuntimeTypeCache).Assembly.Location))
            .Concat(additionalReferences);
        var syntaxTree = CSharpSyntaxTree.ParseText(
            source,
            new CSharpParseOptions(LanguageVersion.CSharp9));

        return CSharpCompilation.Create(
            assemblyName,
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }

    static IEnumerable<MetadataReference> GetFrameworkReferences() {
        var trustedPlatformAssemblies = AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string;
        if (!string.IsNullOrEmpty(trustedPlatformAssemblies)) {
            return trustedPlatformAssemblies!
                .Split(Path.PathSeparator)
                .Where(path => !Path.GetFileName(path).StartsWith("HCommons.", StringComparison.OrdinalIgnoreCase))
                .Select(path => MetadataReference.CreateFromFile(path));
        }

        var assemblyLocations = new List<string> {
            typeof(object).Assembly.Location,
            typeof(Enumerable).Assembly.Location,
            typeof(Attribute).Assembly.Location,
            typeof(IReadOnlyList<>).Assembly.Location,
        };

        var netstandardReference = typeof(RuntimeTypeCache).Assembly
            .GetReferencedAssemblies()
            .FirstOrDefault(reference => reference.Name == "netstandard");
        if (netstandardReference is not null) {
            assemblyLocations.Add(Assembly.Load(netstandardReference).Location);
        }

        return assemblyLocations
            .Where(path => !string.IsNullOrEmpty(path))
            .Distinct()
            .Select(path => MetadataReference.CreateFromFile(path));
    }

    sealed class GeneratorResult {
        public GeneratorResult(
            CSharpCompilation outputCompilation,
            string? generatedSource,
            ImmutableArray<Diagnostic> diagnostics) {
            OutputCompilation = outputCompilation;
            GeneratedSource = generatedSource;
            Diagnostics = diagnostics;
        }

        public CSharpCompilation OutputCompilation { get; }

        public string? GeneratedSource { get; }

        public ImmutableArray<Diagnostic> Diagnostics { get; }
    }
}
