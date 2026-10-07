namespace LunaticPanel.Core.Extensions;

public record EngineBusMsgResponseWithData<TData> : EngineBusMsgResponseNoData
{
    public TData Data { get; init; }

    public EngineBusMsgResponseWithData(TData data, Type componentType, string origin) : base(componentType, origin)
    {
        Data = data;
    }

}
