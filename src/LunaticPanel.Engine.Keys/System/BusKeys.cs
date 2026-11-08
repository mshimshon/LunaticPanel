namespace LunaticPanel.Engine.Keys.System;

public static class BusKeys
{
    public static class Queries
    {
        public const string FETCH_AVAILABLE_BUSES = $"{LPEngineKeys.QUERYPREFIX_V1}.{nameof(BusKeys)}.{nameof(Queries)}.{nameof(FETCH_AVAILABLE_BUSES)}";
    }
}
