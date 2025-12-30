using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Presentation.Layout.Menu.Models;
using LunaticPanel.Engine.Presentation.Layout.Menu.QueryDto.Responses;

namespace LunaticPanel.Engine.Presentation.Layout.Menu.ViewModels;

public class MainMenuViewModel
{
    private readonly IEngineBus _engineBus;
    public IReadOnlyCollection<MenuElementModel> MenuItems { get; set; } = new List<MenuElementModel>();
    public Func<Task> SpreadChanges { get; set; }
    private bool _loading = true;
    public bool Loading
    {
        get => _loading;
        set
        {
            bool refresh = value != _loading;
            _loading = value;
            if (refresh)
                _ = SpreadChanges?.Invoke();
        }
    }

    public MainMenuViewModel(IEngineBus engineBus)
    {
        _engineBus = engineBus;

    }

    public async Task LoadAsync()
    {
        await GetPluginMenuItems();
        Loading = false;
    }

    public async Task GetPluginMenuItems()
    {
        await Task.Delay(1000);
        try
        {
            var message = new EngineBusMessage(new(MainMenuQueries.GetElements));
            var response = await _engineBus.ExecAsync(message);
            MenuItems = response
                .Select(p => (p.RenderFragment, p.Data!.GetDataAs<MenuElementResponse>()!))
                .Select(p => new MenuElementModel() { Position = p.Item2.Position, Render = p.RenderFragment })
                .OrderBy(p => p.Position)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }


    }
}
