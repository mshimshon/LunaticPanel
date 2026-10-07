using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.PluginValidator;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Plugin.Entities;
using LunaticPanel.Engine.Plugin.Validation;
using LunaticPanel.Package.Tool.Exceptions;
using LunaticPanel.Package.Tool.Exceptions.ValidateExceptions;
using LunaticPanel.Package.Tool.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using LunaticPanel.Package.Tool.Tools.Packing;
using LunaticPanel.Package.Tool.Tools.Plugin;
using System.CommandLine;
using System.Reflection;

namespace LunaticPanel.Package.Tool.Tools.Validation;

internal static class PluginValidatorCommandExt
{
    public static RootCommand WithValidateCommands(this RootCommand root)
    {
        var command = new Command("validate", "Test if plugin passes basic validation required.")
            .AddOption<string>("input", "in", "this is the folder of the built plugin or lpkg directly.")
            .SetExecuteCommand(ValidateAction);

        return root.WithSubCommand(command);
    }
    private static async Task<PluginManifestPayload> ValidateAction(ParseResult parseResult, CancellationToken ct = default)
    {
        var input = parseResult.GetValue<string>("--input");
        bool missingParams = input == default;
        if (missingParams)
            throw new MissingParametersException("--input is missing and required for packing.");
        else if (!Directory.Exists(input) && !File.Exists(input))
            throw new ValidateInputInvalidException(input!);
        return await ValidatePackageAsync(input);

    }

    public static async Task<PluginManifestPayload> ValidatePackageAsync(string input)
    {
        bool isPackage = !Directory.Exists(input) && File.Exists(input);
        var inputFolder = input;
        string? outputPackageTmp = default;
        PluginScannedEntity? entity = default;
        PluginManifestPayload? manifest = default;
        if (isPackage)
        {
            Console.Out.WriteLine($"Validating Package for {input}");
            manifest = PluginUtilitiesExt.ReadManifestFromArchive(input);
            var rootTmp = Path.GetTempPath();
            var tmpFolder = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
            outputPackageTmp = Path.Combine(rootTmp, $"lunaticpanel.lpkg.{PackSettings.LunaticPanelVersion}", tmpFolder);
            await PackingCommandExt.UnpackToLocation(manifest, input, outputPackageTmp, true);
            inputFolder = outputPackageTmp;
        }

        try
        {
            Console.Out.WriteLine($"Validating Plugin for {inputFolder}");
            var dll = PluginScannerExt.FindPluginDllInDirectory(inputFolder, [], DependencySettings.ScanSharedFrameworkNames());
            if (dll == default)
                throw new PluginDllNotFoundException(inputFolder);
            LibraryValidatorExt.RunPluginDllValidator(dll);
            manifest = PluginUtilitiesExt.GetManifestInformation(inputFolder);
            Console.Out.WriteLine($"We Found {dll}".Green());

            // No Plugins can dependent on other plugins directly.
            Console.Out.WriteLine($"Checking Hard Dependencies for {dll}".Cyan());

            var hasValidDependencyRule = DependenciesValidationExt.ValidateNoHardDependencies(inputFolder);
            if (!hasValidDependencyRule)
                throw new PluginHardDependencyViolationException(inputFolder);
            Console.Out.WriteLine($"Checking if {inputFolder} has duplicated implementation for PluginBase/IPlugin.".Cyan());

            var hasDuplicatedIPlugin = DependenciesValidationExt.ValidateNoIPluginDuplicates(dll);
            if (!hasDuplicatedIPlugin)
                throw new PluginDuplicateEntryPointException();
            Console.Out.WriteLine($"Test Plugin Loader for {dll}".Cyan());

            entity = PluginScannerExt.LoadPluginInformation(dll, [], DependencySettings.ScanSharedFrameworkNames());
            if (entity == default)
                throw new PluginLoadFailedException(dll);

            var name = AssemblyName.GetAssemblyName(entity.Location);
            var version = name.Version;
            var pluginAsm = entity.Loader.Load();
            Console.Out.WriteLine($"Creating Entry Point for {entity.PluginId}".Cyan());
            var plugin = entity.CreateEntryPoint(pluginAsm);
            Console.Out.WriteLine($"Performing Internal Validation for {entity.PluginId}".Cyan());

            IReadOnlyCollection<Core.Abstraction.Diagnostic.Messages.PluginValidationResult>? pluginValidation = plugin.PerformValidation();
            if (pluginValidation.Any(p => p.Errors?.Count > 0))
            {
                var error = pluginValidation.First(p => !p.Passed).Errors!.First();
                Console.Error.WriteLine($"Validation failed for {entity.PluginId} with {error.Message} ".Red());

                throw new PluginValidationFailedException(plugin.PluginId, error);
            }
            return manifest;
        }
        catch
        {

            throw;
        }
        finally
        {
            if (isPackage)
            {
                try
                {
                    if (entity != default && entity.Loader.IsLoaded)
                    {
                        entity.Loader.Unload();
                        Console.Out.WriteLine($"{entity.PluginId} Unloaded".Green());
                    }

                    if (Directory.Exists(outputPackageTmp))
                    {
                        Directory.Delete(outputPackageTmp, true);
                        Console.Out.WriteLine($"Removed {outputPackageTmp}".Green());

                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Failed to Remove {outputPackageTmp}".Red());
                    Console.Error.WriteLine(ex.Message.Red());

                }
            }
        }

    }
}
