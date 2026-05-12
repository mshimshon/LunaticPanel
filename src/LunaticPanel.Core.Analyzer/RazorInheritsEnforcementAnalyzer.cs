using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace LunaticPanel.Plugin.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RazorInheritsEnforcementAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor MissingInherits = new(
        id: "LPRZ001",
        title: "@inherits is required",
        messageFormat: "Razor component '{0}' must declare exactly one @inherits directive.",
        category: "LunaticPanel.Razor",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor InvalidInherits = new(
        id: "LPRZ002",
        title: "Invalid @inherits base type",
        messageFormat: "Razor component '{0}' uses an invalid base type in @inherits: '{1}'.",
        category: "LunaticPanel.Razor",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(MissingInherits, InvalidInherits);

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.RegisterAdditionalFileAction(AnalyzeRazorFile);
    }

    private static void AnalyzeRazorFile(AdditionalFileAnalysisContext context)
    {
        var path = context.AdditionalFile.Path;

        if (!path.EndsWith(".razor", StringComparison.OrdinalIgnoreCase))
            return;

        var text = context.AdditionalFile.GetText(context.CancellationToken);
        if (text is null)
            return;

        var content = text.ToString();

        var inheritsLines = content
            .Split('\n')
            .Select(l => l.Trim())
            .Where(l => l.StartsWith("@inherits ", StringComparison.Ordinal))
            .ToArray();

        var fileName = Path.GetFileName(path);

        // Rule: exactly one @inherits
        if (inheritsLines.Length != 1)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    MissingInherits,
                    Location.Create(path, default, default),
                    fileName));
            return;
        }

        var inheritsLine = inheritsLines[0];
        var baseType = inheritsLine.Substring("@inherits ".Length).Trim();

        // Allowlist rule (TEXTUAL, intentional)
        if (!IsAllowedBaseType(baseType))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    InvalidInherits,
                    Location.Create(path, default, default),
                    fileName,
                    baseType));
        }
    }

    private static bool IsAllowedBaseType(string baseType)
    {
        // STRICT allowlist – extend if needed
        // Examples:
        // WidgetComponentBase<TPlugin, TViewModel>
        // WidgetComponentBase<MyPlugin, IMyViewModel>

        return baseType.StartsWith(
            "WidgetComponentBase<",
            StringComparison.Ordinal);
    }
}
