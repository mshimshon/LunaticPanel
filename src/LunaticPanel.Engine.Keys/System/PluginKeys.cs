namespace LunaticPanel.Engine.Keys.System;

public static class PluginKeys
{
    public static class Queries
    {
        public const string FetchAll = $"{LPEngineKeys.QueryPrefix_V1}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FetchAll)}";
        /// <summary>
        /// Return List of BootstrapPluginDescriptor from bootstrap file, loaded from disk every single time.
        /// </summary>
        public const string FetchBootstrapFile = $"{LPEngineKeys.QueryPrefix_V1}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FetchBootstrapFile)}";
    }

    public static class Events
    {
        public const string OnInitialize = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnInitialize)}";
        public const string OnParameterSet = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnParameterSet)}";
        public const string OnAfterRender = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnAfterRender)}";
        /// <summary>
        /// Set Plugin as Enabled for the next startup.
        /// </summary>
        public const string EnablePlugin = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(EnablePlugin)}";
        /// <summary>
        /// Set Plugin as Disabled for the next startup.
        /// </summary>
        public const string DisablePlugin = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(DisablePlugin)}";
        /// <summary>
        /// Called after the Plugin is Enabled (Doesn't called if the plugin was disabled directly into the bootstrap.json)
        /// </summary>
        public const string OnPluginEnabled = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnPluginEnabled)}";
        /// <summary>
        /// Called after the Plugin is Disabled (Doesn't called if the plugin was disabled directly into the bootstrap.json)
        /// </summary>
        public const string OnPluginDisabled = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(OnPluginDisabled)}";
    }
}