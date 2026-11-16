using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Package.Tool.Exceptions;
using LunaticPanel.Package.Tool.Exceptions.InfoExceptions;
using LunaticPanel.Package.Tool.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using LunaticPanel.Package.Tool.Tools.Plugin;
using System.CommandLine;

namespace LunaticPanel.Package.Tool.Tools.Info;

internal static class VersionCommandExt
{


    public static RootCommand WithVersionCommand(this RootCommand root)
    {
        var command = new Command("version", "print debug tool information")
            .SetExecuteCommand(ValidateInfo);
        return root.WithSubCommand(command);
    }

    private static Task<ToolInformationPayload> ValidateInfo(ParseResult parseResult, CancellationToken ct = default)
    {
        var result = new ToolInformationPayload()
        {
            BoundToPanelVersion = typeof(IPlugin).Assembly.GetName().Version!.ToString(3)!,
            ToolVersion = typeof(VersionCommandExt).Assembly.GetName().Version!.ToString(3)!
        };

        return Task.FromResult(result);

    }
}
