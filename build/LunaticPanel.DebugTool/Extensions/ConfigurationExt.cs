using LunaticPanel.DebugTool.Payloads;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LunaticPanel.DebugTool.Extensions;

internal static class ConfigurationExt
{
    private static string ComposeTarget { get; set; }
    private static void ValidateParameters(string[] args)
    {
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
        return new ConfigurationPayload()
        {
            WorkingDir = workingDir,
            SkipSubSystemRebuild = args.Any(p => p.Equals("--skip-wsl", StringComparison.OrdinalIgnoreCase)),
            SkipServiceRebuild = args.Any(p => p.Equals("--skip-services", StringComparison.OrdinalIgnoreCase)),
            Compose = new()
            {
                Services = LoadServiceCompose(ComposeTarget),
                Plugins = LoadPluginCompose(ComposeTarget),
                Apt = LoadAptCompose(),
            }
        };
    }
    private static List<PluginComposePayload> LoadPluginCompose(string composeLocation)
    {
        Console.WriteLine($"Loading Plugins from {composeLocation}");
        var workingDir = Path.GetDirectoryName(composeLocation)!;
        if (LoadedPluginComposer.Contains(composeLocation))
            throw new Exception("Compose Import Circular Dependencies.");
        LoadedPluginComposer.Add(composeLocation);
        LoadedComposer.Add(composeLocation);
        var compose = LoadCompose(composeLocation);
        List<PluginComposePayload> imported = new();
        foreach (var plugin in compose.Plugins)
        {
            if (plugin.ImportFrom != default)
            {
                var nextCompose = Path.GetFullPath(plugin.ImportFrom, workingDir);
                var nextDict = LoadPluginCompose(composeLocation);
                foreach (var nextItem in nextDict)
                {
                    if (imported.Contains(nextItem))
                        throw new Exception($"Plugin Import Duplicated Key {nextItem}");
                    imported.Add(nextItem);
                }
                Console.Out.WriteLine($"Imported Plugins From {composeLocation}");
                continue;
            }
            if (imported.Contains(plugin))
                throw new Exception($"Plugin Import Duplicated Key {plugin}");
            imported.Add(ProcessPlugin(plugin, workingDir));
        }
        return imported;
    }
    private static Dictionary<string, ServiceComposePayload> LoadServiceCompose(string composeLocation)
    {
        Console.WriteLine($"Loading Services from {composeLocation}");

        var workingDir = Path.GetDirectoryName(composeLocation)!;
        if (LoadedServiceComposer.Contains(composeLocation))
            throw new Exception("Compose Import Circular Dependencies.");
        LoadedServiceComposer.Add(composeLocation);
        LoadedComposer.Add(composeLocation);
        var compose = LoadCompose(composeLocation);
        Dictionary<string, ServiceComposePayload> importedServices = new();
        foreach (var service in compose.Services)
        {
            Console.WriteLine($"{service.Value}");

            if (service.Value.ImportFrom != default)
            {
                var nextCompose = Path.GetFullPath(service.Value.ImportFrom, workingDir);
                Console.WriteLine($"Import Services from {nextCompose}");
                var nextDict = LoadServiceCompose(nextCompose);
                foreach (var nextItem in nextDict)
                {
                    if (importedServices.ContainsKey(nextItem.Key))
                        throw new Exception($"Service Import Duplicated Key {nextItem.Key}");
                    importedServices[nextItem.Key] = nextItem.Value;
                }
                Console.Out.WriteLine($"Imported Services From {service.Value.ImportFrom}");
                continue;
            }
            if (importedServices.ContainsKey(service.Key))
                throw new Exception($"Service Import Duplicated Key {service.Key}");
            importedServices[service.Key] = ProcessService(service.Value, workingDir);
            Console.WriteLine($"{service.Key} Service Added!");
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
        return apt;
    }

    private static ComposePayload LoadCompose(string composeLocation)
    {
        var workingDir = Path.GetDirectoryName(composeLocation)!;

        var composeFileLoaded = File.ReadAllText(composeLocation);
        Console.WriteLine($"Reading YAML from {composeLocation}");

        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
        Console.WriteLine($"Parsing YAML from {composeLocation}");
        ComposePayload compose = deserializer.Deserialize<ComposePayload>(composeFileLoaded);
        if (compose == default)
            throw new Exception($"'{ComposeTarget}' could not deserialize properly. ");
        Console.WriteLine($"YAML from {composeLocation} Loaded");
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
            ExecStart = Path.Combine(p.WorkingDir, p.ExecStart).Replace('\\', '/').Replace("/./", "/")
        };
    }

    private static PluginComposePayload ProcessPlugin(PluginComposePayload p, string workingDir)
    {
        return p with
        {
            Local = p.Local != default ? Path.GetFullPath(Path.Combine(workingDir, p.Local)) : default
        };
    }

}
