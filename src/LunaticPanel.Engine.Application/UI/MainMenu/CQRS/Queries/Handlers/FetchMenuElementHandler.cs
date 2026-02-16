using CoreMap;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Application.UI.MainMenu.CQRS.Queries.Dto.Responses;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using MedihatR;
namespace LunaticPanel.Engine.Application.UI.MainMenu.CQRS.Queries.Handlers;

internal class FetchMenuElementHandler : IRequestHandler<FetchMenuElementQuery, List<EngineBusMsgResponseWithData<MenuElementEntity>>>
{
    private readonly ICoreMap _coreMap;
    private readonly IEngineBus _engineBus;

    public FetchMenuElementHandler(ICoreMap coreMap, IEngineBus engineBus)
    {
        _coreMap = coreMap;
        _engineBus = engineBus;
    }
    public async Task<List<EngineBusMsgResponseWithData<MenuElementEntity>>> Handle(FetchMenuElementQuery request, CancellationToken cancellationToken)
    {
        List<EngineBusMsgResponseWithData<MenuElementEntity>> result = new();
        try
        {
            var responses = await _engineBus
                .Execute(MainMenuKeys.UI.GetElements)
                .ReadWithData((response) => _coreMap.Map((response.Data?.GetDataAs<MenuElementResponse>()!)).To<MenuElementEntity>(), p => p);
            result = responses.Select(p => p)
                .OrderBy(p => p.Data.Position)
                .ToList();
            Console.WriteLine($"FetchMenuElementHandler::Handle = {result.Count}");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"FetchMenuElementHandler::Handle = {ex.Message}");
        }
        return result;
    }
}
