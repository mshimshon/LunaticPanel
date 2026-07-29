namespace LunaticPanel.Core.PluginValidator;

public static class CoreDependencies
{
    public static string[] OptionalCoreAssemblies { get; } = {
        "LunaticPanel.Core",
        "LunaticPanel.Core.Extensions",
        "LunaticPanel.Core.Utils",
        "LunaticPanel.Core.Utils.Abstraction"
    };

    public static string[] RequiredCoreAssemblies { get; } = {
        "LunaticPanel.Core.Abstraction"
    };
}
