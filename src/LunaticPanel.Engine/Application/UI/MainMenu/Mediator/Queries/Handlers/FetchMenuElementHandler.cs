using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Application.UI.MainMenu.Mediator.Queries;
using LunaticPanel.Engine.Application.UI.MainMenu.Mediator.Queries.Dto.Responses;
using LunaticPanel.Engine.Application.UI.MainMenu.Mediator.Queries.Dto.Responses.Mapping;
using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using LunaticPanel.Engine.Keys.UI;
using MaksimShimshon.Mediator;
namespace LunaticPanel.Engine.Application.UI.MainMenu.Mediator.Queries.Handlers;

internal class FetchMenuElementHandler : IRequestHandler<FetchMenuElementQuery, List<EngineBusMsgResponseWithData<MenuElementEntity>>>
{
    private readonly IEngineBus _engineBus;

    public FetchMenuElementHandler(IEngineBus engineBus)
    {
        _engineBus = engineBus;
    }
    public async Task<List<EngineBusMsgResponseWithData<MenuElementEntity>>> HandleAsync(FetchMenuElementQuery request, CancellationToken cancellationToken)
    {
        List<EngineBusMsgResponseWithData<MenuElementEntity>> result = new();
        try
        { // _coreMap.MapTo<MenuElementResponse, MenuElementEntity>
            var responses = await _engineBus
                .Execute(MainMenuKeys.UI.GET_ELEMENTS)
                .ReadWithData((response) => (response.Data?.GetDataAs<MenuElementResponse>() ?? new()).ToEntity(), p => p);
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
