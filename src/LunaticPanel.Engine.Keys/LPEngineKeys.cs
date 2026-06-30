namespace LunaticPanel.Engine.Keys;

public static partial class LPEngineKeys
{
    public const string AssemblyName = "LunaticPanel.Engine";
    internal const string EventPrefix = $"eventkey.[{AssemblyName}]";
    internal const string EnginePrefix = $"enginekey.[{AssemblyName}]";
    internal const string QueryPrefix = $"querykey.[{AssemblyName}]";

    internal const string EventPrefix_V1 = $"{EventPrefix}.{V1}";
    internal const string EnginePrefix_V1 = $"{EnginePrefix}.{V1}";
    internal const string QueryPrefix_V1 = $"{QueryPrefix}.{V1}";
    internal const string V1 = "v1";

}
