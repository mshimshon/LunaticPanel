using LunaticPanel.Engine.Core;

namespace LunaticPanel.Engine.Keys.System;

public static class PluginKeys
{
    public static class Queries
    {
        public const string FetchAll = $"{BaseInfo.AssemblyName}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FetchAll)}";
    }

    public static class Events
    {
        public const string OnInitialize = $"{BaseInfo.AssemblyName}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnInitialize)}";
        public const string OnParameterSet = $"{BaseInfo.AssemblyName}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnParameterSet)}";
        public const string OnAfterRender = $"{BaseInfo.AssemblyName}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnAfterRender)}";
    }
}