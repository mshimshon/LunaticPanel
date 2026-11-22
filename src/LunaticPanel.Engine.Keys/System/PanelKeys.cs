namespace LunaticPanel.Engine.Keys.System;

public static class PanelKeys
{
    public static class Events
    {
        /// <summary>
        /// Will trigger the auto update cycle to occur early (avoid spamming)
        /// </summary>
        public const string CHECK_PANEL_UPDATE = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(CHECK_PANEL_UPDATE)}";

        /// <summary>
        /// Design to Shutdown the host panel, using systemctl service if configured properly panel should auto restart.
        /// </summary>
        public const string PANEL_SHUTDOWN = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(PANEL_SHUTDOWN)}";

        /// <summary>
        /// Sends Data string of the found latest version available
        /// </summary>
        public const string ON_PANEL_UPDATE_AVAILABLE = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(ON_PANEL_UPDATE_AVAILABLE)}";


        /// <summary>
        /// Sends Data string of the found latest version downloaded
        /// </summary>
        public const string ON_PANEL_UPDATE_DOWNLOADED = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(ON_PANEL_UPDATE_DOWNLOADED)}";

        /// <summary>
        /// Sends version string of the staged incoming update (ONLY OCCURS IF AUTO-RESTART IS OFF OR IF USERS CURRENTLY USING THE PANEL DASHBOARD)
        /// </summary>
        public const string ON_PANEL_UPDATE_STAGED = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(ON_PANEL_UPDATE_STAGED)}";

        public static class ScheduledEvents
        {
            public const string AUTO_UPDATE_CYCLE = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(ScheduledEvents)}.{nameof(AUTO_UPDATE_CYCLE)}";

        }
    }
    public static class Queries
    {
        /// <summary>
        /// Return Version String of the current panel.
        /// </summary>
        public const string GET_PANEL_VERSION = $"{LPEngineKeys.QUERYPREFIX_V1}.{nameof(Queries)}.{nameof(Queries)}.{nameof(GET_PANEL_VERSION)}";

    }
}
