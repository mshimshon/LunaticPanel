namespace LunaticPanel.Engine.Keys;

public static partial class LPEngineKeys
{
    public const string ASSEMBLY_NAME = "LunaticPanel.Engine";
    internal const string EVENT_PREFIX = $"eventkey.[{ASSEMBLY_NAME}]";
    internal const string ENGINE_PREFIX = $"enginekey.[{ASSEMBLY_NAME}]";
    internal const string QUERY_PREFIX = $"querykey.[{ASSEMBLY_NAME}]";

    internal const string EVENTPREFIX_V1 = $"{EVENT_PREFIX}.{V1}";
    internal const string ENGINEPREFIX_V1 = $"{ENGINE_PREFIX}.{V1}";
    internal const string QUERYPREFIX_V1 = $"{QUERY_PREFIX}.{V1}";
    internal const string V1 = "v1";

}
