using LunaticPanel.Package.Tool.Exceptions;
using LunaticPanel.Package.Tool.Exceptions.InfoExceptions;
using LunaticPanel.Package.Tool.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using LunaticPanel.Package.Tool.Tools.Plugin;
using System.CommandLine;

namespace LunaticPanel.Package.Tool.Tools.Info;

internal static class InfoCommandExt
{


    public static RootCommand WithInfoCommands(this RootCommand root)
    {
        var command = new Command("info", "extract manifest info from plugin archive")
            .AddOption<string>("input", "in", "this is 'pluginid.version.lpkg' file.")
            .SetExecuteCommand(ValidateInfo);
        return root.WithSubCommand(command);
    }

    private static Task<PluginManifestPayload> ValidateInfo(ParseResult parseResult, CancellationToken ct = default)
    {
        var input = parseResult.GetValue<string>("--input");
        bool missingParams = input == default;
        if (missingParams)
            throw new MissingParametersException("--input is missing and required for packing.");
        else if (!File.Exists(input))
            throw new InfoInputFileNotExistException();
        var result = PluginUtilitiesExt.ReadManifestFromArchive(input);
        return Task.FromResult(result);

    }
}
