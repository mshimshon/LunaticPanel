using LunaticPanel.DebugTool.Payloads;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LunaticPanel.DebugTool.Extensions;

internal static class ConfigurationExt
{
    private static string ComposeTarget { get; set; }
    private static void ValidateParameters(string[] args)
    {
        string ymlExt = Path.Combine(Environment.CurrentDirectory, "lpcli-compose.yml");
        string yamlExt = Path.Combine(Environment.CurrentDirectory, "lpcli-compose.yaml");
        bool foundLocalYML = File.Exists(ymlExt) || File.Exists(yamlExt);
        if (!foundLocalYML)
            throw new Exception($"'{Environment.CurrentDirectory}' doesn't contain a 'lpcli-compose.yml' ");
        ComposeTarget = File.Exists(ymlExt) ? ymlExt : yamlExt;
    }

    public static ConfigurationPayload GenerateConfiguration(string[] args)
    {
        ValidateParameters(args);
        var deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build();
        var compose = deserializer.Deserialize<ComposePayload>(File.ReadAllText(ComposeTarget));
        if (compose == default)
            throw new Exception($"'{ComposeTarget}' could not deserialize properly. ");

        return new ConfigurationPayload()
        {
            SkipSubSystemRebuild = args.Contains("--quick"),
            Compose = compose
        };
    }

}
