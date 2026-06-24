namespace LunaticPanel.Engine.Keys.System;

public static class PluginKeys
{
    public static class Queries
    {
        public const string FetchAll = $"{BaseInfo.QueryPrefix}.{BaseInfo.AssemblyName}.{BaseInfo.V1}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FetchAll)}";
    }

    public static class Events
    {
        public const string OnInitialize = $"{BaseInfo.EventPrefix}.{BaseInfo.AssemblyName}.{BaseInfo.V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnInitialize)}";
        public const string OnParameterSet = $"{BaseInfo.EventPrefix}.{BaseInfo.AssemblyName}.{BaseInfo.V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnParameterSet)}";
        public const string OnAfterRender = $"{BaseInfo.EventPrefix}.{BaseInfo.AssemblyName}.{BaseInfo.V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnAfterRender)}";
    }
}