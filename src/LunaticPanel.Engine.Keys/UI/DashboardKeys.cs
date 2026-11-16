namespace LunaticPanel.Engine.Keys.UI;

public static class DashboardKeys
{

    public static class UI
    {
        public const string GET_WIDGETS = $"{LPEngineKeys.ENGINEPREFIX_V1}.{nameof(DashboardKeys)}.{nameof(UI)}.{nameof(GET_WIDGETS)}";
    }

    public static class Events
    {
        public const string ON_FIRST_RENDER = $"{LPEngineKeys.EVENTPREFIX_V1}.{nameof(DashboardKeys)}.{nameof(Events)}.{nameof(ON_FIRST_RENDER)}";

    }
}
