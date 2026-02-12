using CoreMap;
using LunaticPanel.Core.Abstraction.Messaging.EngineBus;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Application.UI.MainMenu.CQRS.Queries.Dto.Responses;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Domain.UI.Menu.Entites;
using MedihatR;
namespace LunaticPanel.Engine.Application.UI.MainMenu.CQRS.Queries.Handlers;

internal class FetchMenuElementHandler : IRequestHandler<FetchMenuElementQuery, List<MenuElementEntity>>
{
    private readonly ICoreMap _coreMap;
    private readonly IEngineBus _engineBus;

    public FetchMenuElementHandler(ICoreMap coreMap, IEngineBus engineBus)
    {
        _coreMap = coreMap;
        _engineBus = engineBus;
    }
    public async Task<List<MenuElementEntity>> Handle(FetchMenuElementQuery request, CancellationToken cancellationToken)
    {
        List<MenuElementEntity> result = new();
        try
        {
            // TODO: CHANGE WHEN COREMAP SUPPORT CTOR
            var responses = await _engineBus
                .Execute(MainMenuKeys.UI.GetElements)
                .ReadWithData((response) =>
                    _coreMap.Map((response.Data!.GetDataAs<MenuElementResponse>()!)).To<MenuElementEntity>() with
                    {
                        ComponentType = response.ComponentType,
                        Render = response.RenderFragment
                    }, p => p);
            result = responses.Select(p => p.Data)
                .OrderBy(p => p.Position)
                .ToList();

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return result;
    }
}
