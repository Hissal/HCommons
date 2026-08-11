using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace HCommons.Reflection.SourceGeneration;

/// <summary>
/// Reports cache requests that cannot be honored because a runtime type filter contains a delegate predicate.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RuntimeTypeFilterAnalyzer : DiagnosticAnalyzer {
    const string FilterTypeName = "HCommons.Reflection.RuntimeTypeFilter";

    static readonly DiagnosticDescriptor s_uncacheableFilter = new(
        "HCRTCFILTER001",
        "Runtime type filter cannot be cached",
        "Caching is ignored because this runtime type filter contains a delegate-based Where predicate",
        "HCommons.Reflection.Filtering",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description:
        "Delegate behavior has no stable structural identity. Use an immutable RuntimeTypeFilterRule when the condition must participate in snapshot caching.");

    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(s_uncacheableFilter);

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context) {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    static void AnalyzeInvocation(SyntaxNodeAnalysisContext context) {
        var invocation = (InvocationExpressionSyntax)context.Node;
        if (context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol is not IMethodSymbol method ||
            method.Name != "Cached" ||
            GetMetadataName(method.ContainingType) != FilterTypeName) {
            return;
        }

        var completeChain = GetCompleteFluentChain(invocation);
        foreach (var nestedInvocation in completeChain.DescendantNodesAndSelf().OfType<InvocationExpressionSyntax>()) {
            if (context.SemanticModel.GetSymbolInfo(nestedInvocation, context.CancellationToken).Symbol is IMethodSymbol nestedMethod &&
                IsDelegateWhere(nestedMethod)) {
                context.ReportDiagnostic(Diagnostic.Create(s_uncacheableFilter, invocation.GetLocation()));
                return;
            }
        }
    }

    static InvocationExpressionSyntax GetCompleteFluentChain(InvocationExpressionSyntax invocation) {
        var current = invocation;

        while (current.Parent is MemberAccessExpressionSyntax memberAccess &&
               ReferenceEquals(memberAccess.Expression, current) &&
               memberAccess.Parent is InvocationExpressionSyntax parentInvocation) {
            current = parentInvocation;
        }

        return current;
    }

    static bool IsDelegateWhere(IMethodSymbol method) {
        if (method.Name != "Where" ||
            method.Parameters.Length != 1 ||
            GetMetadataName(method.ContainingType) is not (FilterTypeName or "HCommons.Reflection.RuntimeTypeFilters")) {
            return false;
        }

        if (method.Parameters[0].Type is not INamedTypeSymbol parameterType ||
            parameterType.Name != "Func" ||
            parameterType.Arity != 2 ||
            GetMetadataName(parameterType.ContainingType) is not null ||
            parameterType.ContainingNamespace.ToDisplayString() != "System") {
            return false;
        }

        return GetMetadataName(parameterType.TypeArguments[0]) == "System.Type" &&
               parameterType.TypeArguments[1].SpecialType == SpecialType.System_Boolean;
    }

    static string? GetMetadataName(ISymbol? symbol) {
        if (symbol is null) {
            return null;
        }

        if (symbol is INamespaceSymbol namespaceSymbol) {
            return namespaceSymbol.IsGlobalNamespace ? string.Empty : namespaceSymbol.ToDisplayString();
        }

        var containingName = GetMetadataName(symbol.ContainingSymbol);
        if (string.IsNullOrEmpty(containingName)) {
            return symbol.MetadataName;
        }

        return containingName + "." + symbol.MetadataName;
    }
}
