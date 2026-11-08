namespace LunaticPanel.Engine.Keys.System;

public static class PluginKeys
{
    public static class Queries
    {
        public const string FETCH_ALL = $"{LPEngineKeys.QUERYPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FETCH_ALL)}";
        /// <summary>
        /// Return List of BootstrapPluginDescriptor from bootstrap file, loaded from disk every single time.
        /// </summary>
        public const string FETCH_BOOTSTRAP_FILE = $"{LPEngineKeys.QUERYPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FETCH_BOOTSTRAP_FILE)}";
        /// <summary>
        /// Same as FetchBootstrapFile but only return the currently loaded version from host RAM. (Used to compare changes)
        /// </summary>
        public const string FETCH_BOOTSTRAP = $"{LPEngineKeys.QUERYPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Queries)}.{nameof(FETCH_BOOTSTRAP)}";
    }

    public static class Events
    {
        public const string ON_INITIALIZE = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(ON_INITIALIZE)}";
        public const string ON_PARAMETER_SET = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(ON_PARAMETER_SET)}";
        public const string ON_AFTER_RENDER = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(ON_AFTER_RENDER)}";
        /// <summary>
        /// Set Plugin as Enabled for the next startup.
        /// </summary>
        public const string ENABLE_PLUGIN = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(ENABLE_PLUGIN)}";
        /// <summary>
        /// Set Plugin as Disabled for the next startup.
        /// </summary>
        public const string DISABLE_PLUGIN = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(DISABLE_PLUGIN)}";
        /// <summary>
        /// Called after the Plugin is Enabled (Doesn't called if the plugin was disabled directly into the bootstrap.json)
        /// </summary>
        public const string ON_PLUGIN_ENABLED = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(ON_PLUGIN_ENABLED)}";
        /// <summary>
        /// Called after the Plugin is Disabled (Doesn't called if the plugin was disabled directly into the bootstrap.json)
        /// </summary>
        public const string ON_PLUGIN_DISABLED = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PluginKeys)}.{nameof(Events)}.{nameof(ON_PLUGIN_DISABLED)}";
    }
}
