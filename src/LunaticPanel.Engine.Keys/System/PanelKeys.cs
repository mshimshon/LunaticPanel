namespace LunaticPanel.Engine.Keys.System;

public static class PanelKeys
{
    public static class Events
    {
        /// <summary>
        /// Triggers the panel to shutdown (should auto restart)
        /// </summary>
        public const string CHECK_PANEL_UPDATE = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(CHECK_PANEL_UPDATE)}";
        public const string PANEL_SHUTDOWN = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(PANEL_SHUTDOWN)}";

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
