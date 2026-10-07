namespace LunaticPanel.Engine.Keys.System;

public static class BusKeys
{
    public static class Queries
    {
        public const string FetchAvailableBuses = $"{LPEngineKeys.QueryPrefix_V1}.{nameof(BusKeys)}.{nameof(Queries)}.{nameof(FetchAvailableBuses)}";
    }
}
