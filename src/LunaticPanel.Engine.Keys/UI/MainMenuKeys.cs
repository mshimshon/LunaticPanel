namespace LunaticPanel.Engine.Keys.UI;

/// <summary>
/// SOLUTION Prefix + PluginID + Version + Discriminator
/// EngineKey.AssemblyName.vX.WHATEVER
/// QueryKey.AssemblyName.vX.WHATEVER
/// EventKey.AssemblyName.vX.WHATEVER
/// </summary>
public static class MainMenuKeys
{
    public static class UI
    {
        public const string GetElements = $"{BaseInfo.AssemblyName}.{nameof(MainMenuKeys)}.{nameof(UI)}.{nameof(GetElements)}";
    }
}
