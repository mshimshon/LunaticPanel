using LunaticPanel.DebugTool.Payloads;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LunaticPanel.DebugTool.Extensions;

internal static class ConfigurationExt
{
    private static string ComposeTarget { get; set; } = default!;

    public static void PrintHelp()
    {
        Console.WriteLine($"lpcli [\"path to .lpcli-compose\" or run directly from compose path]");
        Console.WriteLine($"lpcli --hard-reset | Run before deploy, Purge the whole temp folder including WSL snapshots and disks.");
        Console.WriteLine($"lpcli --no-open | Prevent the opening of services or terminal after depoy.");
        Console.WriteLine($"lpcli --open-only \"service_name,service_name,service_name\" | Open specific services and ignore all others.");
        Console.WriteLine($"lpcli --open-shell | open a interactable shell at deploy");
        Console.WriteLine($"lpcli --clean | Run before deploy, selectively clean all the temp folder from build, pack, publish artifacts! it can safely be ran everytime.");
        Console.WriteLine($"lpcli --deploy | execute steps from compose file and deploy services + plugins.");
        Console.WriteLine($"lpcli --no-interaction | prevent user input requirement and auto allow everything.");
        Console.WriteLine($"lpcli --debug | print all information in cmd.");
        Console.WriteLine($"lpcli --skip-apt | after initial deploy subsequents deploy will skip rebuilding the base Debian system and installing apt dependencies.");
        Console.WriteLine($"lpcli --skip-services | prevent subsequents deploy from rebuilding services and directly jump to plugin deployment.");
        Console.WriteLine($"lpcli --auto-kill | Automatically kill processes opened at the deploy time when CTRL+C.");
        Console.WriteLine($"lpcli --snap \"name\" | start executing steps from the snapshot step in compose.");
    }
    private static void ValidateParameters(string[] args)
    {
        PrintHelp();
        string workingDir = Environment.CurrentDirectory;
        if (args.Length > 0 && Directory.Exists(args[0]))
            workingDir = args[0];
        string ymlExt = Path.Combine(workingDir, "lpcli-compose.yml");
        string yamlExt = Path.Combine(workingDir, "lpcli-compose.yaml");
        bool foundLocalYML = File.Exists(ymlExt) || File.Exists(yamlExt);
        if (!foundLocalYML)
            throw new Exception($"'{workingDir}' doesn't contain a 'lpcli-compose.yml' ");
        ComposeTarget = File.Exists(ymlExt) ? ymlExt : yamlExt;
    }
    private static HashSet<string> LoadedComposer { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    private static List<string> LoadedServiceComposer { get; set; } = new();
    private static List<string> LoadedPluginComposer { get; set; } = new();
    private static List<string> LoadedPPComposer { get; set; } = new();
    private static Action<string> PrintDebug { get; set; } = (s) => { };
    private static bool DebugMode { get; set; }

    private static string? GetParameterValue(this string[] args, string key)
    {
        int snapIndex = args.IndexOf(key, StringComparer.OrdinalIgnoreCase);
        if (snapIndex >= 0)
        {
            return args[snapIndex + 1];
        }
        return default;
    }

    private static bool HasFlag(this string[] args, string key)
     => args.Any(p => p.Equals(key, StringComparison.OrdinalIgnoreCase));
    public static ConfigurationPayload GenerateConfiguration(string[] args)
    {
        string workingDir = Environment.CurrentDirectory;
        if (args.Length > 0 && Directory.Exists(args[0]))
            workingDir = args[0];
        ValidateParameters(args);
        foreach (var item in args)
        {
            Console.Out.WriteLine($"Parameter: {item}");
        }
        DebugMode = args.HasFlag("--debug");
        string? snap = args.GetParameterValue("--snap");
        string[] targetedOpens = args.GetParameterValue("--open-only")?.Split(',') ?? Array.Empty<string>();


        var result = new ConfigurationPayload()
        {
            WorkingDir = workingDir,
            DebugMode = DebugMode,
            Snap = snap,
            OpenOnly = targetedOpens,
            AutoKill = args.HasFlag("--auto-kill"),
            NoOpen = args.HasFlag("--no-open"),
            OpenShell = args.HasFlag("--open-shell"),
            SkipSubSystemRebuild = args.HasFlag("--skip-apt"),
            PerformSoftCleanup = args.HasFlag("--clean"),
            PerformCleanup = args.HasFlag("--hard-reset"),
            PerformDeploy = args.HasFlag("--deploy"),
            SkipServiceRebuild = args.HasFlag("--skip-services"),
            NoInteraction = args.HasFlag("--no-interaction"),

        };
        PrintDebug = result.PrintDebug;

        result = result with
        {
            Compose = new()
            {
                Services = LoadServiceCompose(ComposeTarget),
                Plugins = LoadPluginCompose(ComposeTarget),
                PostProcessing = LoadPostProcessingCompose(ComposeTarget),
                Apt = LoadAptCompose(),
            }
        };


        return result;
    }
    private static List<PostProcessingComposePayload> LoadPostProcessingCompose(string composeLocation, string? chainFrom = default)
    {
        PrintDebug($"Loading PP from {composeLocation}");
        var workingDir = Path.GetDirectoryName(composeLocation)!;

        if (LoadedPPComposer.Contains(composeLocation))
            throw new Exception($"[Circular Dependencies]: {(chainFrom != default ? $"'{chainFrom}' ->" : "")}' '{composeLocation}'.");
        LoadedPPComposer.Add(composeLocation);
        LoadedComposer.Add(composeLocation);
        var compose = LoadCompose(composeLocation);
        List<PostProcessingComposePayload> imported = new();
        foreach (var pp in compose.PostProcessing)
        {
            if (pp.ImportFrom != default)
            {
                var nextCompose = Path.GetFullPath(pp.ImportFrom, workingDir);
                var nextDict = LoadPostProcessingCompose(nextCompose, composeLocation);
                foreach (var nextItem in nextDict)
                {
                    if (imported.Contains(nextItem))
                    {
                        PrintDebug($"PP Import Duplicated Key {nextItem}");
                        continue;
                    }
                    imported.Add(nextItem);
                }
                PrintDebug($"Imported PP From {composeLocation}");
                continue;
            }
            if (imported.Contains(pp))
            {
                PrintDebug($"PP Duplicated Key {pp}");
                continue;
            }
            var processed = ProcessPostProcessingJob(pp, workingDir);
            imported.Add(processed);
            PostProcessingJobValidation(processed);
        }
        return imported;
    }

    private static List<PluginComposePayload> LoadPluginCompose(string composeLocation, string? chainFrom = default)
    {
        PrintDebug($"Loading Plugins from {composeLocation}");
        var workingDir = Path.GetDirectoryName(composeLocation)!;
        if (LoadedPluginComposer.Contains(composeLocation))
            throw new Exception($"[Circular Dependencies]: {(chainFrom != default ? $"'{chainFrom}' ->" : "")}' '{composeLocation}'.");
        LoadedPluginComposer.Add(composeLocation);
        LoadedComposer.Add(composeLocation);
        var compose = LoadCompose(composeLocation);
        List<PluginComposePayload> imported = new();
        foreach (var plugin in compose.Plugins)
        {
            if (plugin.ImportFrom != default)
            {
                var nextCompose = Path.GetFullPath(plugin.ImportFrom, workingDir);
                var nextDict = LoadPluginCompose(nextCompose, composeLocation);
                foreach (var nextItem in nextDict)
                {
                    if (imported.Contains(nextItem))
                    {
                        PrintDebug($"Plugin Import Duplicated Key {nextItem}");
                        continue;
                    }
                    imported.Add(nextItem);
                }
                PrintDebug($"Imported Plugins From {composeLocation}");
                continue;
            }
            if (imported.Contains(plugin))
            {
                PrintDebug($"Plugin Duplicated Key {plugin}");
                continue;
            }
            var processed = ProcessPlugin(plugin, workingDir);
            imported.Add(processed);
            PluginValidation(processed);
        }
        return imported;
    }
    private static List<ServiceComposePayload> LoadServiceCompose(string composeLocation, string? chainFrom = default)
    {
        PrintDebug($"Loading Services from {composeLocation}");

        var workingDir = Path.GetDirectoryName(composeLocation)!;
        if (LoadedServiceComposer.Contains(composeLocation))
            throw new Exception($"[Circular Dependencies]: {(chainFrom != default ? $"'{chainFrom}' ->" : "")}' '{composeLocation}'.");
        LoadedServiceComposer.Add(composeLocation);
        LoadedComposer.Add(composeLocation);
        var compose = LoadCompose(composeLocation);
        List<ServiceComposePayload> importedServices = new();
        foreach (var service in compose.Services)
        {
            PrintDebug($"{service}");

            if (service.ImportFrom != default)
            {
                var nextCompose = Path.GetFullPath(service.ImportFrom, workingDir);
                PrintDebug($"Import Services from {nextCompose}");
                var nextDict = LoadServiceCompose(nextCompose, composeLocation);
                foreach (var nextItem in nextDict)
                {
                    if (importedServices.Any(p => p.ServiceName == nextItem.ServiceName))
                        throw new Exception($"Service Import Duplicated Key {nextItem.ServiceName}");
                    importedServices.Add(nextItem);
                }
                Console.Out.WriteLine($"Imported Services From {service.ImportFrom}");
                continue;
            }
            if (importedServices.Any(p => p.ServiceName == service.ServiceName))
                throw new Exception($"Service Import Duplicated Key {service.ServiceName!}");
            var processed = ProcessService(service, workingDir);
            importedServices.Add(processed);
            ServiceValidation(processed);
            PrintDebug($"{service.ServiceName} Service Added!");
        }
        return importedServices;
    }
    private static HashSet<string> LoadAptCompose()
    {
        HashSet<string> apt = new(StringComparer.OrdinalIgnoreCase);
        foreach (var item in LoadedComposer)
        {
            var compose = LoadCompose(item);
            apt.UnionWith(compose.Apt);
        }
        PrintDebug($"APT Install Found -> {string.Join(',', apt)}");
        return apt;
    }

    private static ComposePayload LoadCompose(string composeLocation)
    {
        var workingDir = Path.GetDirectoryName(composeLocation)!;
        if (!File.Exists(composeLocation))
            throw new Exception($"{composeLocation}' was not found.");
        var composeFileLoaded = File.ReadAllText(composeLocation);
        PrintDebug($"Reading YAML from {composeLocation}");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        PrintDebug($"Parsing YAML from {composeLocation}");
        ComposePayload compose = deserializer.Deserialize<ComposePayload>(composeFileLoaded);
        if (compose == default)
            throw new Exception($"'{ComposeTarget}' could not deserialize properly. ");
        Console.WriteLine($"YAML from {composeLocation} Loaded");

        if (DebugMode)
        {
            var serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
                .Build();
            var composeSrting = serializer.Serialize(compose);
            PrintDebug(composeSrting);
        }
        return compose;
        //compose = compose with
        //{
        //    Plugins = compose.Plugins.Select(p => p with
        //    {
        //        Local = p.Local != default ? Path.GetFullPath(Path.Combine(workingDir, p.Local)) : default
        //    }).ToList(),
        //    Services = compose.Services.Select(p => ProcessService(p, workingDir)).ToDictionary()
        //};
    }

    private static ServiceComposePayload ProcessService(ServiceComposePayload p, string workingDir)
    {
        return p with
        {
            DotnetProject = p.DotnetProject != default ? Path.GetFullPath(Path.Combine(workingDir, p.DotnetProject)) : default,
            ExecStart = Path.Combine(p.WorkingDir!, p.ExecStart!).Replace('\\', '/').Replace("/./", "/")
            // TODO: TEST GET FULLPATH
        };
    }

    private static PluginComposePayload ProcessPlugin(PluginComposePayload p, string workingDir)
    {
        return p with
        {
            DotnetProject = p.DotnetProject != default ? Path.GetFullPath(Path.Combine(workingDir, p.DotnetProject)) : default
        };
    }

    private static PostProcessingComposePayload ProcessPostProcessingJob(PostProcessingComposePayload p, string workingDir)
    {
        return p with
        {

            DotnetProject = p.DotnetProject != default ? Path.GetFullPath(Path.Combine(workingDir, p.DotnetProject)) : default,
            File = p.File != default ? Path.GetFullPath(Path.Combine(workingDir, p.File)) : default,
            Folder = p.Folder != default ? Path.GetFullPath(Path.Combine(workingDir, p.Folder)) : default,
        };
    }

    private static void PluginValidation(PluginComposePayload p)
    {
        bool isDotnet = !string.IsNullOrWhiteSpace(p.DotnetProject);
        bool isSource = !string.IsNullOrWhiteSpace(p.Id) || !string.IsNullOrWhiteSpace(p.Source);
        bool isSnap = !string.IsNullOrWhiteSpace(p.Snap);
        if (isSnap) return;
        if (!isDotnet && !isSource)
            throw new Exception($"Plugin must either be dotnet project or plugin source + id.");
        if (isDotnet && !File.Exists(p.DotnetProject))
            throw new Exception($"{p.DotnetProject} does not exist.");
        if (isSource && string.IsNullOrWhiteSpace(p.Id))
            throw new Exception($"Plugin id is required to query source {p.Source}.");
        if (isSource && string.IsNullOrWhiteSpace(p.Source))
            throw new Exception($"Plugin {p.Id} does not have a source set.");
    }

    private static void ServiceValidation(ServiceComposePayload p)
    {
        bool isDotnet = !string.IsNullOrWhiteSpace(p.DotnetProject);
        bool isDeb = !string.IsNullOrWhiteSpace(p.DebUrl);
        if (!isDotnet && !isDeb)
            throw new Exception($"Service must either be dotnet project or deb url.");
        if (isDotnet && !File.Exists(p.DotnetProject))
            throw new Exception($"{p.DotnetProject} does not exist.");
        if (string.IsNullOrWhiteSpace(p.ServiceName))
            throw new Exception($"({nameof(p.ServiceName)}) is required {p}.");
        if (string.IsNullOrWhiteSpace(p.WorkingDir))
            throw new Exception($"Service '{p.ServiceName}' ({nameof(p.WorkingDir)}) is required.");
        if (string.IsNullOrWhiteSpace(p.ExecStart))
            throw new Exception($"Service '{p.ServiceName}' ({nameof(p.ExecStart)}) is required.");
    }

    private static void PostProcessingJobValidation(PostProcessingComposePayload p)
    {
        bool isSnap = !string.IsNullOrWhiteSpace(p.Snap);
        if (isSnap) return;

        bool isDotnet = !string.IsNullOrWhiteSpace(p.DotnetProject) || !string.IsNullOrWhiteSpace(p.BuildTo) || !string.IsNullOrWhiteSpace(p.PublishTo) || !string.IsNullOrWhiteSpace(p.PluginPackTo);
        bool isCopyFolder = !string.IsNullOrWhiteSpace(p.Folder) || !string.IsNullOrWhiteSpace(p.FolderTo);
        bool isCommand = !string.IsNullOrWhiteSpace(p.Command);
        if (isCommand) return;
        bool isCopyFile = !string.IsNullOrWhiteSpace(p.File) || !string.IsNullOrWhiteSpace(p.FileTo);
        bool isArchive = !string.IsNullOrWhiteSpace(p.Archive);
        if (!isDotnet && !isCopyFolder && !isCopyFile)
            throw new Exception($"Post Processing must copy folder, file, linux command or build dotnet project... {p}");
        if (isDotnet && string.IsNullOrWhiteSpace(p.DotnetProject))
            throw new Exception($"Dotnet project not spefcified {p}.");
        if (isDotnet && !File.Exists(p.DotnetProject))
            throw new Exception($"{p.DotnetProject} does not exist.");
        if (!string.IsNullOrWhiteSpace(p.DotnetProject) && string.IsNullOrWhiteSpace(p.BuildTo) && string.IsNullOrWhiteSpace(p.PublishTo) && string.IsNullOrWhiteSpace(p.PluginPackTo))
            throw new Exception($"{p.DotnetProject} does not have a WSL build or publish or PluginPackTo target. {p}");

        if (isCopyFolder && string.IsNullOrWhiteSpace(p.FolderTo))
            throw new Exception($"{p.Folder} Copy Content Where???.");
        if (isCopyFolder && string.IsNullOrWhiteSpace(p.Folder))
            throw new Exception($"{p.FolderTo} Copy Content From???.");
        if (isCopyFile && string.IsNullOrWhiteSpace(p.FileTo))
            throw new Exception($"{p.File} Copy File Where???.");
        if (isCopyFile && string.IsNullOrWhiteSpace(p.File))
            throw new Exception($"{p.FileTo} Copy File From???.");
        if (isArchive && string.IsNullOrWhiteSpace(p.FolderTo) && string.IsNullOrWhiteSpace(p.PublishTo) && string.IsNullOrWhiteSpace(p.BuildTo)
            && string.IsNullOrWhiteSpace(p.FileTo))
            throw new Exception($"Archive has no destination {p}.");
        if (isArchive && p.Archive != "tar.gz" && p.Archive != "zip")
            throw new Exception($"Archive has no valid type 'zip', 'tar.gz' {p}.");
    }

}
