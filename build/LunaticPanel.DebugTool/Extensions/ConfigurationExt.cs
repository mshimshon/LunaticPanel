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

    public static ConfigurationPayload GenerateConfiguration(string[] args)
    {
        string workingDir = Environment.CurrentDirectory;
        if (args.Length > 0 && Directory.Exists(args[0]))
            workingDir = args[0];
        ValidateParameters(args);
        var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
        var compose = deserializer.Deserialize<ComposePayload>(File.ReadAllText(ComposeTarget));
        if (compose == default)
            throw new Exception($"'{ComposeTarget}' could not deserialize properly. ");
        foreach (var item in args)
        {
            Console.Out.WriteLine($"Parameter: {item}");
        }
        return new ConfigurationPayload()
        {
            WorkingDir = workingDir,
            SkipSubSystemRebuild = args.Any(p => p.Equals("--skip-wsl", StringComparison.OrdinalIgnoreCase)),
            SkipServiceRebuild = args.Any(p => p.Equals("--skip-services", StringComparison.OrdinalIgnoreCase)),
            Compose = compose with
            {
                Plugins = compose.Plugins.Select(p => p with
                {
                    Local = p.Local != default ? Path.GetFullPath(Path.Combine(workingDir, p.Local)) : default
                }).ToList(),
                Services = compose.Services.Select(p => new KeyValuePair<string, ServiceComposePayload>(p.Key, p.Value with
                {
                    DotnetProject = p.Value.DotnetProject != default ? Path.GetFullPath(Path.Combine(workingDir, p.Value.DotnetProject)) : default,
                    ExecStart = Path.Combine(p.Value.WorkingDir, p.Value.ExecStart).Replace('\\', '/').Replace("/./", "/")

                })).ToDictionary()
            }
        };
    }

}
