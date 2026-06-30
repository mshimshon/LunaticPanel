namespace LunaticPanel.Engine.Keys.System;

public static class PluginKeys
{
    public static class Queries
    {
        public const string FetchAll = $"{LPEngineKeys.QueryPrefix_V1}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FetchAll)}";
    }

    public static class Events
    {
        public const string OnInitialize = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnInitialize)}";
        public const string OnParameterSet = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnParameterSet)}";
        public const string OnAfterRender = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnAfterRender)}";
    }
}