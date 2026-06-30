using LunaticPanel.Package.Tool.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using LunaticPanel.Package.Tool.Tools.Plugin;
using LunaticPanel.Package.Tool.Tools.Validation;
using System.CommandLine;
using System.IO.Compression;
namespace LunaticPanel.Package.Tool.Tools.Packing;

internal static class PackingCommandExt
{

    private static string[] _excludedDlls = new string[]
    {
        "LunaticPanel.Core",
        "LunaticPanel.Core.Abstraction",
        "LunaticPanel.Core.Analyzer",
        "LunaticPanel.Core.Extensions",
        "LunaticPanel.Core.PluginValidator",
        "LunaticPanel.Core.Utils",
        "LunaticPanel.Core.Utils.Abstraction",
        "LunaticPanel.Engine.Application",
        "LunaticPanel.Engine.Domain",
        "LunaticPanel.Engine.Infrastructure",
        "LunaticPanel.Engine.Web",
        "LunaticPanel.Hybrid.Web",
        "MudBlazor",
        "Microsoft.AspNetCore.Antiforgery",
        "Microsoft.AspNetCore.Authentication.Abstractions",
        "Microsoft.AspNetCore.Authentication.BearerToken",
        "Microsoft.AspNetCore.Authentication.Cookies",
        "Microsoft.AspNetCore.Authentication.Core",
        "Microsoft.AspNetCore.Authentication",
        "Microsoft.AspNetCore.Authentication.OAuth",
        "Microsoft.AspNetCore.Authorization",
        "Microsoft.AspNetCore.Authorization.Policy",
        "Microsoft.AspNetCore.Components.Authorization",
        "Microsoft.AspNetCore.Components",
        "Microsoft.AspNetCore.Components.Endpoints",
        "Microsoft.AspNetCore.Components.Forms",
        "Microsoft.AspNetCore.Components.Server",
        "Microsoft.AspNetCore.Components.Web",
        "Microsoft.AspNetCore.Connections.Abstractions",
        "Microsoft.AspNetCore.CookiePolicy",
        "Microsoft.AspNetCore.Cors",
        "Microsoft.AspNetCore.Cryptography.Internal",
        "Microsoft.AspNetCore.Cryptography.KeyDerivation",
        "Microsoft.AspNetCore.DataProtection.Abstractions",
        "Microsoft.AspNetCore.DataProtection",
        "Microsoft.AspNetCore.DataProtection.Extensions",
        "Microsoft.AspNetCore.Diagnostics.Abstractions",
        "Microsoft.AspNetCore.Diagnostics",
        "Microsoft.AspNetCore.Diagnostics.HealthChecks",
        "Microsoft.AspNetCore",
        "Microsoft.AspNetCore.HostFiltering",
        "Microsoft.AspNetCore.Hosting.Abstractions",
        "Microsoft.AspNetCore.Hosting",
        "Microsoft.AspNetCore.Hosting.Server.Abstractions",
        "Microsoft.AspNetCore.Html.Abstractions",
        "Microsoft.AspNetCore.Http.Abstractions",
        "Microsoft.AspNetCore.Http.Connections.Common",
        "Microsoft.AspNetCore.Http.Connections",
        "Microsoft.AspNetCore.Http",
        "Microsoft.AspNetCore.Http.Extensions",
        "Microsoft.AspNetCore.Http.Features",
        "Microsoft.AspNetCore.Http.Results",
        "Microsoft.AspNetCore.HttpLogging",
        "Microsoft.AspNetCore.HttpOverrides",
        "Microsoft.AspNetCore.HttpsPolicy",
        "Microsoft.AspNetCore.Identity",
        "Microsoft.AspNetCore.Localization",
        "Microsoft.AspNetCore.Localization.Routing",
        "Microsoft.AspNetCore.Metadata",
        "Microsoft.AspNetCore.Mvc.Abstractions",
        "Microsoft.AspNetCore.Mvc.ApiExplorer",
        "Microsoft.AspNetCore.Mvc.Core",
        "Microsoft.AspNetCore.Mvc.Cors",
        "Microsoft.AspNetCore.Mvc.DataAnnotations",
        "Microsoft.AspNetCore.Mvc",
        "Microsoft.AspNetCore.Mvc.Formatters.Json",
        "Microsoft.AspNetCore.Mvc.Formatters.Xml",
        "Microsoft.AspNetCore.Mvc.Localization",
        "Microsoft.AspNetCore.Mvc.Razor",
        "Microsoft.AspNetCore.Mvc.RazorPages",
        "Microsoft.AspNetCore.Mvc.TagHelpers",
        "Microsoft.AspNetCore.Mvc.ViewFeatures",
        "Microsoft.AspNetCore.OutputCaching",
        "Microsoft.AspNetCore.RateLimiting",
        "Microsoft.AspNetCore.Razor",
        "Microsoft.AspNetCore.Razor.Runtime",
        "Microsoft.AspNetCore.RequestDecompression",
        "Microsoft.AspNetCore.ResponseCaching.Abstractions",
        "Microsoft.AspNetCore.ResponseCaching",
        "Microsoft.AspNetCore.ResponseCompression",
        "Microsoft.AspNetCore.Rewrite",
        "Microsoft.AspNetCore.Routing.Abstractions",
        "Microsoft.AspNetCore.Routing",
        "Microsoft.AspNetCore.Server.HttpSys",
        "Microsoft.AspNetCore.Server.IIS",
        "Microsoft.AspNetCore.Server.IISIntegration",
        "Microsoft.AspNetCore.Server.Kestrel.Core",
        "Microsoft.AspNetCore.Server.Kestrel",
        "Microsoft.AspNetCore.Server.Kestrel.Transport.NamedPipes",
        "Microsoft.AspNetCore.Server.Kestrel.Transport.Quic",
        "Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets",
        "Microsoft.AspNetCore.Session",
        "Microsoft.AspNetCore.SignalR.Common",
        "Microsoft.AspNetCore.SignalR.Core",
        "Microsoft.AspNetCore.SignalR",
        "Microsoft.AspNetCore.SignalR.Protocols.Json",
        "Microsoft.AspNetCore.StaticAssets",
        "Microsoft.AspNetCore.StaticFiles",
        "Microsoft.AspNetCore.WebSockets",
        "Microsoft.AspNetCore.WebUtilities",
        "Microsoft.CSharp",
        "Microsoft.Extensions.Caching.Abstractions",
        "Microsoft.Extensions.Caching.Memory",
        "Microsoft.Extensions.Configuration.Abstractions",
        "Microsoft.Extensions.Configuration.Binder",
        "Microsoft.Extensions.Configuration.CommandLine",
        "Microsoft.Extensions.Configuration",
        "Microsoft.Extensions.Configuration.EnvironmentVariables",
        "Microsoft.Extensions.Configuration.FileExtensions",
        "Microsoft.Extensions.Configuration.Ini",
        "Microsoft.Extensions.Configuration.Json",
        "Microsoft.Extensions.Configuration.KeyPerFile",
        "Microsoft.Extensions.Configuration.UserSecrets",
        "Microsoft.Extensions.Configuration.Xml",
        "Microsoft.Extensions.DependencyInjection.Abstractions",
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.Diagnostics.Abstractions",
        "Microsoft.Extensions.Diagnostics",
        "Microsoft.Extensions.Diagnostics.HealthChecks.Abstractions",
        "Microsoft.Extensions.Diagnostics.HealthChecks",
        "Microsoft.Extensions.Features",
        "Microsoft.Extensions.FileProviders.Abstractions",
        "Microsoft.Extensions.FileProviders.Composite",
        "Microsoft.Extensions.FileProviders.Embedded",
        "Microsoft.Extensions.FileProviders.Physical",
        "Microsoft.Extensions.FileSystemGlobbing",
        "Microsoft.Extensions.Hosting.Abstractions",
        "Microsoft.Extensions.Hosting",
        "Microsoft.Extensions.Http",
        "Microsoft.Extensions.Identity.Core",
        "Microsoft.Extensions.Identity.Stores",
        "Microsoft.Extensions.Localization.Abstractions",
        "Microsoft.Extensions.Localization",
        "Microsoft.Extensions.Logging.Abstractions",
        "Microsoft.Extensions.Logging.Configuration",
        "Microsoft.Extensions.Logging.Console",
        "Microsoft.Extensions.Logging.Debug",
        "Microsoft.Extensions.Logging",
        "Microsoft.Extensions.Logging.EventLog",
        "Microsoft.Extensions.Logging.EventSource",
        "Microsoft.Extensions.Logging.TraceSource",
        "Microsoft.Extensions.ObjectPool",
        "Microsoft.Extensions.Options.ConfigurationExtensions",
        "Microsoft.Extensions.Options.DataAnnotations",
        "Microsoft.Extensions.Options",
        "Microsoft.Extensions.Primitives",
        "Microsoft.Extensions.Validation",
        "Microsoft.Extensions.WebEncoders",
        "Microsoft.JSInterop",
        "Microsoft.Net.Http.Headers",
        "Microsoft.VisualBasic.Core",
        "Microsoft.VisualBasic",
        "Microsoft.Win32.Primitives",
        "Microsoft.Win32.Registry"
    }.Select(p => p.ToLower()).ToArray();
    public static RootCommand WithPackCommands(this RootCommand root, IServiceProvider serviceProvider)
    {
        var command = new Command("pack", "pack plugin folder to .lpkg")
            .AddOption<string>("input", "in", "this is the input folder of the plugin.")
            .AddOption<string>("output", "out", "where to write the lpkg.")
            .SetPackingAction(serviceProvider);
        return root.WithSubCommand(command);
    }
    public static RootCommand WithUnPackCommands(this RootCommand root, IServiceProvider serviceProvider)
    {
        var command = new Command("unpack", "pack plugin folder to .lpkg")
            .AddOption<string>("input", "in", "this lpkg file.")
            .AddOption<string>("output", "out", "where to write files.")
            .SetPackingAction(serviceProvider);
        return root.WithSubCommand(command);
    }
    public static bool IsDirectoryPath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        char last = path[path.Length - 1];

        // Explicit directory indicator
        if (last == Path.DirectorySeparatorChar || last == Path.AltDirectorySeparatorChar)
            return true;

        // If last segment has no dot, assume directory
        string name = Path.GetFileName(path);
        return !name.Contains('.');
    }

    private static Command SetPackingAction(this Command command, IServiceProvider serviceProvider)
    {
        command.SetAction(async (parseResult, ct) =>
        {
            var serviceModControl = serviceProvider.GetRequiredService<IEngineModService>();
            bool success = false;
            var input = parseResult.GetValue<string>("--input");
            var output = parseResult.GetValue<string>("--output");
            bool missingParams = input == default || output == default;
            if (missingParams)
                await command.PrintHelp();
            else if (!IsDirectoryPath(input!))
            {
                var outResult = new ResultResponse()
                {
                    Error = new ErrorResponse("PackInputNotDirectory", $"'{input}' is not a valid directory.")
                };
                await outResult.PrintAsync();
            }
            else if (!Directory.Exists(input))
            {
                var outResult = new ResultResponse()
                {
                    Error = new ErrorResponse("PackDirectoryMissing", $"'{input}' is missing.")
                };
                await outResult.PrintAsync();
            }

            else if (!IsDirectoryPath(output!))
            {
                var outResult = new ResultResponse()
                {
                    Error = new ErrorResponse("PackOutputNotDirectory", $"'{output}' is not a valid directory.")
                };
                await outResult.PrintAsync();
            }
            else
                success = await PackAsync();

            if (success)
                Environment.Exit(0);
            else
                Environment.Exit(1);
        });
        return command;
    }

    public static async Task<bool> PackAsync(string input, string output)
    {
        bool isValid = await PluginValidatorCommandExt.ValidatePackageAsync(input);
        if (!isValid)
            return false;

    }


    public static void PackLpkg(string inputFolder, string output)
    {
        if (!Directory.Exists(inputFolder))
            throw new DirectoryNotFoundException(inputFolder);

        string plugin = PluginUtilitiesExt.FindPluginEntryFile(inputFolder);
        string version = PluginUtilitiesExt.GetAssemblyVersion(plugin);
        string pluginName = Path.GetFileNameWithoutExtension(plugin);
        string outputPackage = Path.Combine(output, $"{pluginName}.{version}.lpkg");
        if (File.Exists(outputPackage))
            File.Delete(outputPackage);

        using var zip = ZipFile.Open(outputPackage, ZipArchiveMode.Create);
        var basePath = Path.GetFullPath(inputFolder);
        var files = Directory.GetFiles(basePath, "*", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            // SKIP Excluded DLL those are host supplied.
            if (_excludedDlls.Contains(Path.GetFileNameWithoutExtension(file).ToLower()))
                continue;

            var fullPath = Path.GetFullPath(file);
            var relative = fullPath.Substring(basePath.Length).TrimStart(Path.DirectorySeparatorChar);
            zip.CreateEntryFromFile(fullPath, relative, CompressionLevel.Optimal);
        }

    }

}
