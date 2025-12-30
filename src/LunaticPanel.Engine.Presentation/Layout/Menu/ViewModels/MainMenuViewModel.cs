using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Core.UI;
using LunaticPanel.Engine.Presentation.Layout.Menu.Models;
using LunaticPanel.Engine.Presentation.Layout.Menu.QueryDto.Responses;
using SwizzleV;

namespace LunaticPanel.Engine.Presentation.Layout.Menu.ViewModels;

public class MainMenuViewModel
{
    private readonly IEngineBus _engineBus;
    public IReadOnlyCollection<MenuElementModel> MenuItems { get; set; } = new List<MenuElementModel>();
    private readonly ISwizzleViewModel _swizzleViewModel;
    private bool _loading = true;
    public bool Loading
    {
        get => _loading;
        set
        {
            bool refresh = value != _loading;
            _loading = value;
            if (refresh)
                _ = _swizzleViewModel.SpreadChanges(() => this);
        }
    }

    public MainMenuViewModel(IEngineBus engineBus, ISwizzleViewModel swizzleViewModel)
    {
        _engineBus = engineBus;
        _swizzleViewModel = swizzleViewModel;

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
