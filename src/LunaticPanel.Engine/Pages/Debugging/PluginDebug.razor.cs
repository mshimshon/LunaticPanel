using LunaticPanel.Engine.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine.Pages.Debugging;

public partial class PluginDebug : ComponentBase
{
    [Inject] public IPluginRegistry PluginRegistry { get; set; } = default!;
    protected override void OnInitialized()
    {

    }
}
