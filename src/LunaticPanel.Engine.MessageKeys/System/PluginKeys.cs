using LunaticPanel.Engine.Core;

namespace LunaticPanel.Engine.Keys.System;

public static class PluginKeys
{
    public static class Queries
    {
        public const string FetchAll = $"{BaseInfo.AssemblyName}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FetchAll)}";
    }
}