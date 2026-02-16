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
    public static async Task<EngineBusMsgResponseWithData<TData>[]> ReadWithData<TData>(this Task<EngineBusResponse[]> executionTask, Func<EngineBusResponse, TData> map,
        Func<EngineBusMsgResponseWithData<TData>, EngineBusMsgResponseWithData<TData>> mapMessage)
    {
        var result = await executionTask;
        return await result.ReadWithData(map, mapMessage);
    }

    private static EngineBusMsgResponseWithData<TData> CreateMessageWithData<TData>(EngineBusResponse response, Func<EngineBusResponse, TData> map)
        => response.ComponentType != default ?
            new EngineBusMsgResponseWithData<TData>(map(response), response.ComponentType, response.Origin) :
            new EngineBusMsgResponseWithData<TData>(map(response), response.RenderFragment!, response.Origin);

    public static async Task<EngineBusMsgResponseWithData<TData>[]> ReadWithData<TData>(
        this EngineBusResponse[] engineBusResponses,
        Func<EngineBusResponse, TData> mapData,
        Func<EngineBusMsgResponseWithData<TData>, EngineBusMsgResponseWithData<TData>> mapMessage)
        =>
        engineBusResponses.Select(p =>
        {
            var r = mapMessage(CreateMessageWithData(p, mapData) with
            {
                VisibilityCondition = p.VisibilityCondition
            });
            return r;
        }).ToArray();

    private static EngineBusMsgResponseNoData CreateMessageNoData(string origin, RenderFragment? render = default, Type? componentType = default)
    => componentType != default ?
        new EngineBusMsgResponseNoData(componentType, origin) :
        new EngineBusMsgResponseNoData(render!, origin);

    public static async Task<EngineBusMsgResponseNoData[]> ReadDiscardData(this Task<EngineBusResponse[]> executionTask)
    {
        var result = await executionTask;
        return await result.ReadDiscardData();
    }
    public static async Task<EngineBusMsgResponseNoData[]> ReadDiscardData(this EngineBusResponse[] engineBusResponses)
    => engineBusResponses.Select(p => CreateMessageNoData(p.Origin, p.RenderFragment, p.ComponentType) with
    {
        VisibilityCondition = p.VisibilityCondition
    }).ToArray();


    private static RenderFragment CreateRenderFragmentComponent<TComponent>() where TComponent : IComponent
    => builder =>
    {
        builder.OpenComponent<TComponent>(0);

        builder.CloseComponent();
    };

    public static Task<EngineBusResponse> ReplyWithFragmentOf<TComponent>(this IEngineBusMessage engineBusMessage) where TComponent : IComponent
    {
        RenderFragment fragment = CreateRenderFragmentComponent<TComponent>();
        var result = new EngineBusResponse(fragment);
        return Task.FromResult(result);
    }
    public static Task<EngineBusResponse> ReplyWithFragmentOf<TComponent>(this IEngineBusMessage engineBusMessage, object data) where TComponent : IComponent
    {
        RenderFragment fragment = CreateRenderFragmentComponent<TComponent>();
        var result = new EngineBusResponse(fragment, data);
        return Task.FromResult(result);
    }

    public static Task<EngineBusResponse> ReplyWithFragmentOf<TComponent>(this IEngineBusMessage engineBusMessage, object data, Func<EngineBusResponse, EngineBusResponse> extra) where TComponent : IComponent
    {
        RenderFragment fragment = CreateRenderFragmentComponent<TComponent>();
        var result = new EngineBusResponse(fragment, data);
        result = extra?.Invoke(result) ?? result;
        return Task.FromResult(result);
    }
    public static Task<EngineBusResponse> ReplyWithFragmentOf<TComponent>(this IEngineBusMessage engineBusMessage, Func<EngineBusResponse, EngineBusResponse> extra) where TComponent : IComponent
    {
        RenderFragment fragment = CreateRenderFragmentComponent<TComponent>();
        var result = new EngineBusResponse(fragment);
        result = extra?.Invoke(result) ?? result;
        return Task.FromResult(result);
    }


    public static Task<EngineBusResponse> ReplyWithTypeOf<TComponent>(this IEngineBusMessage engineBusMessage) where TComponent : IComponent
    {
        RenderFragment fragment = CreateRenderFragmentComponent<TComponent>();
        var result = new EngineBusResponse(typeof(TComponent));
        return Task.FromResult(result);
    }
    public static Task<EngineBusResponse> ReplyWithTypeOf<TComponent>(this IEngineBusMessage engineBusMessage, object data) where TComponent : IComponent
    {
        RenderFragment fragment = CreateRenderFragmentComponent<TComponent>();
        var result = new EngineBusResponse(typeof(TComponent), data);
        return Task.FromResult(result);
    }

    public static Task<EngineBusResponse> ReplyWithTypeOf<TComponent>(this IEngineBusMessage engineBusMessage, object data, Func<EngineBusResponse, EngineBusResponse> extra) where TComponent : IComponent
    {
        RenderFragment fragment = CreateRenderFragmentComponent<TComponent>();
        var result = new EngineBusResponse(typeof(TComponent), data);
        result = extra?.Invoke(result) ?? result;
        return Task.FromResult(result);
    }
    public static Task<EngineBusResponse> ReplyWithTypeOf<TComponent>(this IEngineBusMessage engineBusMessage, Func<EngineBusResponse, EngineBusResponse> extra) where TComponent : IComponent
    {
        var result = new EngineBusResponse(typeof(TComponent));
        result = extra?.Invoke(result) ?? result;
        return Task.FromResult(result);
    }
}
