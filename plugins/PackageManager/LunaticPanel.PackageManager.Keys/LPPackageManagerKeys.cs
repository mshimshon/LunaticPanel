namespace LunaticPanel.PackageManager.Keys;

public static class LPPackageManagerKeys
{
    public const string ASSEMBLY_NAME = "LunaticPanel.PackageManager";
    public const string MODULE_NAME = "packagemanager";
    public const string API_V1 = "v1";
    internal const string EVENT_PREFIX = $"eventkey.[{ASSEMBLY_NAME}]";
    internal const string ENGINE_PREFIX = $"enginekey.[{ASSEMBLY_NAME}]";
    internal const string QUERY_PREFIX = $"querykey.[{ASSEMBLY_NAME}]";

    internal const string V1 = "v1";
    internal const string EVENTPREFIX_V1 = $"{EVENT_PREFIX}.{V1}";
    internal const string ENGINEPREFIX_V1 = $"{ENGINE_PREFIX}.{V1}";
    internal const string QUERYPREFIX_V1 = $"{QUERY_PREFIX}.{V1}";

    public static class Event
    {
        public static class Scheduled
        {
            public const string PACKAGE_UPDATE_SCHEDULE = $"{EVENTPREFIX_V1}.{nameof(Scheduled)}.PackageUpdateSchedule";
        }
    }
}
