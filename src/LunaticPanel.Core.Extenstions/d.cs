using LunaticPanel.Core.Messaging.Common;
using LunaticPanel.Core.Messaging.EngineBus;

namespace LunaticPanel.Core.Extenstions;

public static class d
{
    //var message = new EngineBusMessage(new(MainMenuQueries.GetElements));
    //var responses = await _engineBus.ExecAsync(message);
    //var responsePair =
    //    responses.Select(p => (p.RenderFragment, p.Data!.GetDataAs<MenuElementResponse>()!));
    //        foreach (var item in responses)
    //        {
    //            try
    //            {
    //                var data = item.Data!.GetDataAs<MenuElementResponse>()!;
    //var menuItem = _coreMap.Map(data).To<MenuElementEntity>() with { Render = item.RenderFragment };
    //result.Add(menuItem);
    //            }
    //            catch (Exception ex)
    //            {
    //                Console.WriteLine(ex.Message);
    //            }
    //        }
    public static async Task<EngineBusResponse[]> Execute(this IEngineBus engineBus, MessageKey key, object? data = default)
    {
        var message = data != default ? new EngineBusMessage(key, data) : new EngineBusMessage(key);
        return await engineBus.ExecAsync(message);
    }

    public static async Task<EngineBusMsgResponseWithData<TData>[]> ReadWithData<TData>(this EngineBusResponse[] engineBusResponses, Func<BusMessageData, TData> map)
        => engineBusResponses.Select(p => new EngineBusMsgResponseWithData<TData>(map(p.Data!), p.RenderFragment, p.Origin)).ToArray();

    public static void EachResponseAs(this EngineBusResponse[] engineBusResponses)
    {
        foreach (var item in engineBusResponses)
        {

        }

    }
}
