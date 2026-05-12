using LunaticPanel.Engine.Keys;

namespace LunaticPanel.Engine.Keys.UI;

public static class DashboardKeys
{
    public static class UI
    {
        public const string GetWidgets = $"{BaseInfo.AssemblyName}.{nameof(DashboardKeys)}.{nameof(UI)}.{nameof(GetWidgets)}";

    }
    public static class Events
    {
        public const string OnFirstRender = $"{BaseInfo.AssemblyName}.{nameof(DashboardKeys)}.{nameof(Events)}.{nameof(OnFirstRender)}";

    }
}
