namespace LunaticPanel.Engine.Keys.System;

public static class PanelKeys
{
    public static class Events
    {
        /// <summary>
        /// Triggers the panel to shutdown (should auto restart)
        /// </summary>
        public const string PanelShutdown = $"{LPEngineKeys.EventPrefix_V1}.{nameof(PanelKeys)}.{nameof(Events)}.{nameof(PanelShutdown)}";
    }
}