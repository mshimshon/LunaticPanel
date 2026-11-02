namespace LunaticPanel.Engine.Keys.System;

public static class PanelKeys
{
    public static class Events
    {
        /// <summary>
        /// Triggers the panel to shutdown (should auto restart)
        /// </summary>
        public const string CheckPanelUpdate = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(CheckPanelUpdate)}";
        public const string PanelShutdown = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(PanelShutdown)}";

        public static class ScheduledEvents
        {
            public const string AutoUpdateCycle = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(ScheduledEvents)}.{nameof(AutoUpdateCycle)}";

        }
    }
    public static class Queries
    {
        /// <summary>
        /// Return Version String of the current panel.
        /// </summary>
        public const string GetPanelVersion = $"{LPEngineKeys.QueryPrefix_V1}.{nameof(Queries)}.{nameof(Queries)}.{nameof(GetPanelVersion)}";

    }
}
