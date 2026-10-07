namespace LunaticPanel.Core.Extensions;

public record EngineBusMsgResponseNoData
{
    public string Origin { get; init; }
    public Func<bool>? VisibilityCondition { get; init; }
    public Type ComponentType { get; }
    public EngineBusMsgResponseNoData(Type componentType, string origin)
    {
        ComponentType = componentType;
        Origin = origin;
    }
}
