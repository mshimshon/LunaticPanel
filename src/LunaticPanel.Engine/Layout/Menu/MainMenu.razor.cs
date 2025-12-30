using LunaticPanel.Core.Messaging.EngineBus;
using LunaticPanel.Engine.Presentation.Layout.Menu.Models;
using LunaticPanel.Engine.Presentation.Layout.Menu.QueryDto.Responses;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Layout.Menu;

public partial class MainMenu : ComponentBase
{
    [Inject] IEngineBus EngineBus { get; set; } = default!;
    ICollection<MenuElementModel> MenuItems { get; set; } = new List<MenuElementModel>();
    protected override void OnInitialized()
    {
        Console.WriteLine("");
    }
    protected override void OnAfterRender(bool firstRender)
    {

        if (firstRender)
        {
            Console.WriteLine("");
        }
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetPluginMenuItems();
        }
    }
    public async Task GetPluginMenuItems()
    {
        try
        {
            var message = new EngineBusMessage("Engine.Layout.Menu", "Fetch");
            var response = await EngineBus.ExecAsync(message);
            MenuItems = response
                .Select(p => (p.RenderFragment, p.Data!.GetDataAs<MenuElementResponse>()!))
                .Select(p => new MenuElementModel() { Position = p.Item2.Position, Render = p.RenderFragment })
                .OrderBy(p => p.Position)
                .ToList();
            await InvokeAsync(StateHasChanged);
        }
        catch (Exception ex)
        {

            throw;
        }


    }
}
