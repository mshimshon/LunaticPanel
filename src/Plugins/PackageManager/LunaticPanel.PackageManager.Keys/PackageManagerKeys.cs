namespace LunaticPanel.PackageManager.Keys;

public static class PackageManagerKeys
{
    public const string AssemblyName = "LunaticPanel.PackageManager";
    public const string API_V1 = "v1";
    public static class Event
    {
        public const string EVENT_KEY = "eventkey";
        public static class Scheduled
        {
            public const string PackageUpdateSchedule = $"{EVENT_KEY}.{AssemblyName}.{API_V1}.{nameof(Scheduled)}.PackageUpdateSchedule";
        }
    }
}
