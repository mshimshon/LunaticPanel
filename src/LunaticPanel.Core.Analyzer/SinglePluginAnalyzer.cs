using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace LunaticPanel.Core.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SinglePluginAnalyzer : DiagnosticAnalyzer
{
    public const string DIAGNOSTIC_ID = "LP001";

    private const string CATEGORY = "Usage";
    private const string PLUGIN_INTERFACE_METADATA_NAME = "LunaticPanel.Core.IPlugin";

    private static readonly DiagnosticDescriptor _rule =
        new DiagnosticDescriptor(
            DIAGNOSTIC_ID,
            "Multiple IPlugin implementations detected",
            "Found {0} implementations of IPlugin across project and references. Only one implementation is allowed: {1}",
            CATEGORY,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(_rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationAction(AnalyzeCompilation);
    }

    private void AnalyzeCompilation(CompilationAnalysisContext context)
    {
        var compilation = context.Compilation;

        var pluginInterface = compilation.GetTypeByMetadataName(PLUGIN_INTERFACE_METADATA_NAME);
        if (pluginInterface == null)
            return;

        // Local implementations in this project
        var local = compilation.GetSymbolsWithName(_ => true, SymbolFilter.Type)
            .OfType<INamedTypeSymbol>()
            .Where(t =>
                t.TypeKind == TypeKind.Class &&
                !t.IsAbstract &&
                t.AllInterfaces.Contains(pluginInterface, SymbolEqualityComparer.Default))
            .ToList();


        var all = new List<INamedTypeSymbol>(local.Count);
        all.AddRange(local);

        if (all.Count <= 1)
            return;

        var names = string.Join(", ", all.Select(t => t.ToDisplayString()));

        // Only report on local source symbols (we can't report on metadata locations)
        foreach (var impl in local)
        {
            foreach (var loc in impl.Locations.Where(l => l.IsInSource))
            {
                var diagnostic = Diagnostic.Create(
                    _rule,
                    loc,
                    all.Count,
                    names);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
