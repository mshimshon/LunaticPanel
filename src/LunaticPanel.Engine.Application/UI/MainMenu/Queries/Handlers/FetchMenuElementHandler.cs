using CoreMap;
using LunaticPanel.Core.Extenstions;
using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Application.UI.MainMenu.Queries.Dto.Responses;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Domain.Menu;
using MedihatR;
namespace LunaticPanel.Engine.Application.UI.MainMenu.Queries.Handlers;

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
            var responses = await _engineBus
                .Execute(MainMenuQueries.GetElements)
                .ReadWithData(msg => _coreMap.Map(msg.GetDataAs<MenuElementResponse>()!).To<MenuElementEntity>());
            foreach (var item in responses)
                try
                {
                    result.Add(item.Data with { Render = item.Render });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        return result.OrderBy(p => p.Position).ToList();
    }
}
