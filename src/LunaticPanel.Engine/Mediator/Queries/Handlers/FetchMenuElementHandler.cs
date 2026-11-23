using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Keys.UI;
using LunaticPanel.Engine.Mediator.Payloads.Responses;
using LunaticPanel.Engine.Mediator.Payloads.Responses.Mapping;
using LunaticPanel.Engine.Mediator.Queries;
using LunaticPanel.Engine.Models.Menu;
using MaksimShimshon.Mediator;
namespace LunaticPanel.Engine.Mediator.Queries.Handlers;

internal class FetchMenuElementHandler : IRequestHandler<FetchMenuElementQuery, List<EngineBusMsgResponseWithData<MenuElementModel>>>
{
    private readonly IEngineBus _engineBus;

    public FetchMenuElementHandler(IEngineBus engineBus)
    {
        _engineBus = engineBus;
    }
    public async Task<List<EngineBusMsgResponseWithData<MenuElementModel>>> HandleAsync(FetchMenuElementQuery request, CancellationToken cancellationToken)
    {
        List<EngineBusMsgResponseWithData<MenuElementModel>> result = new();
        try
        { // _coreMap.MapTo<MenuElementResponse, MenuElementEntity>
            var responses = await _engineBus
                .Execute(MainMenuKeys.UI.GET_ELEMENTS)
                .ReadWithData((response) => (response.Data?.GetDataAs<MenuElementResponse>() ?? new()).ToModel(), p => p);
            result = responses
                .OrderBy(p => p.Data.Position)
                .ToList();
            Console.WriteLine($"FetchMenuElementHandler::Handle = {result.Count}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"FetchMenuElementHandler::Exception = {ex.Message}");
        }
        return result;
    }
}
