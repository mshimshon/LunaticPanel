using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Extensions;

public static class EngineBusExt
{
    //var responses = await _engineBus.Execute(MainMenuQueries.GetElements).ReadWithData(msg=>_coreMap.Map(msg).To<MenuElementEntity>());
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
    public static async Task<EngineBusResponse[]> Execute(this IEngineBus engineBus, string key, object? data = default)
    {
        var message = data != default ? new EngineBusMessage(new(key), data) : new EngineBusMessage(new(key));
        return await engineBus.ExecAsync(message);
    }
    public static async Task<EngineBusMsgResponseWithData<TData>[]> ReadWithData<TData>(this Task<EngineBusResponse[]> executionTask, Func<BusMessageData, TData> map)
    {
        var result = await executionTask;
        return await result.ReadWithData(map);
    }

    public static async Task<EngineBusMsgResponseWithData<TData>[]> ReadWithData<TData>(this EngineBusResponse[] engineBusResponses, Func<BusMessageData, TData> map)
        => engineBusResponses.Select(p => new EngineBusMsgResponseWithData<TData>(map(p.Data!), p.RenderFragment, p.Origin)
        {
            VisibilityCondition = p.VisibilityCondition
        }).ToArray();

    public static async Task<EngineBusMsgResponseNoData[]> ReadDiscardData(this Task<EngineBusResponse[]> executionTask)
    {
        var result = await executionTask;
        return await result.ReadDiscardData();
    }
    public static async Task<EngineBusMsgResponseNoData[]> ReadDiscardData(this EngineBusResponse[] engineBusResponses)
    => engineBusResponses.Select(p => new EngineBusMsgResponseNoData(p.RenderFragment, p.Origin)
    {
        VisibilityCondition = p.VisibilityCondition
    }).ToArray();

    public static Task<EngineBusResponse> ReplyWith<TComponent>(this IEngineBusMessage engineBusMessage, object? data = default) where TComponent : IComponent
    {
        RenderFragment fragment = builder =>
        {
            builder.OpenComponent<TComponent>(0);
            builder.CloseComponent();
        };
        var result = data == default ? new EngineBusResponse(fragment) : new EngineBusResponse(fragment, data);
        return Task.FromResult(result);
    }

    public static Task<EngineBusResponse> ReplyWith<TComponent>(this IEngineBusMessage engineBusMessage, object? data = default, Func<EngineBusResponse, EngineBusResponse>? extra = default) where TComponent : IComponent
    {
        RenderFragment fragment = builder =>
        {
            builder.OpenComponent<TComponent>(0);
            builder.CloseComponent();
        };
        var result = data == default ? new EngineBusResponse(fragment) : new EngineBusResponse(fragment, data);
        result = extra?.Invoke(result) ?? result;
        return Task.FromResult(result);
    }


}
