namespace LunaticPanel.PackageManager.Keys;

public static class LPPackageManagerKeys
{
    public const string AssemblyName = "LunaticPanel.PackageManager";
    public const string API_V1 = "v1";
    internal const string EventPrefix = $"eventkey.[{AssemblyName}]";
    internal const string EnginePrefix = $"enginekey.[{AssemblyName}]";
    internal const string QueryPrefix = $"querykey.[{AssemblyName}]";

    internal const string V1 = "v1";
    internal const string EventPrefix_V1 = $"{EventPrefix}.{V1}";
    internal const string EnginePrefix_V1 = $"{EnginePrefix}.{V1}";
    internal const string QueryPrefix_V1 = $"{QueryPrefix}.{V1}";

    public static class Event
    {
        public static class Scheduled
        {
            public const string PackageUpdateSchedule = $"{EventPrefix_V1}.{nameof(Scheduled)}.PackageUpdateSchedule";
        }
    }
}
