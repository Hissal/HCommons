using System.Collections.Immutable;
using System.Reflection;
using HCommons.Reflection;
using HCommons.Reflection.SourceGeneration;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace HCommons.Tests;

public sealed class RuntimeTypeCacheGeneratorTests {
    [Fact]
    public void Generator_DiscoversGenericAndTypeOfCallsThroughMethodMetadata() {
        const string source = """
            using System;
            using System.Collections.Generic;
            using Cache = HCommons.Reflection.RuntimeTypeCache;

            public interface IMarker { }
            public abstract class MarkerBase : IMarker { }
            internal sealed class Marker : MarkerBase { }

            public static class Queries {
                public static void Run() {
                    _ = Cache.TypesDerivedFrom<IMarker>();
                    _ = Cache.TypesDerivedFrom(typeof(IMarker));
                    _ = Cache.Bind<IMarker>(_ => { });
                    _ = Cache.Bind(typeof(IMarker), _ => { });
                }
            }
            """;

        var result = RunGenerator(source);

        result.GeneratedSource!.ShouldContain("typeof(global::IMarker), true");
        result.GeneratedSource!.ShouldContain("typeof(global::MarkerBase)");
        result.GeneratedSource!.ShouldContain("typeof(global::Marker)");
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
