namespace LunaticPanel.Engine.Keys.UI;

public static class DashboardKeys
{

    public static class UI
    {
        public const string GetWidgets = $"{BaseInfo.EnginePrefix}.{BaseInfo.AssemblyName}.{BaseInfo.V1}.{nameof(DashboardKeys)}.{nameof(UI)}.{nameof(GetWidgets)}";
    }

    public static class Events
    {
        public const string OnFirstRender = $"{BaseInfo.EventPrefix}.{BaseInfo.AssemblyName}.{BaseInfo.V1}.{nameof(DashboardKeys)}.{nameof(Events)}.{nameof(OnFirstRender)}";

    }
}
