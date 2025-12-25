using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Layout.Menu.Models;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Layout.Menu;

public partial class MainMenu : ComponentBase
{
    [Inject] IEngineBus EngineBus { get; set; } = default!;
    ICollection<MenuElementModel> MenuItems { get; set; } = new List<MenuElementModel>();
    public async Task GetPluginMenuItems()
    {
        var message = new EngineBusMessage("Engine.Layout.Menu", "Fetch");
        var response = await EngineBus.ExecAsync(message);
        MenuItems = response
            .Select(p => (p.RenderFragment, p.Data!.GetDataAs<MenuItemMetadataDto>()!))
            .Select(p => new MenuElementModel() { Position = p.Item2.Position, Render = p.RenderFragment })
            .OrderBy(p => p.Position)
            .ToList();
        await InvokeAsync(StateHasChanged);
    }


}
