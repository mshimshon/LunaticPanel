using LunaticPanel.Engine.Bootstraper;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Engine;

public partial class Routes : ComponentBase
{
    [Inject] public IBootstrap Bootstrap { get; set; } = default!;
    protected override void OnInitialized()
    {
        try
        {
            // your logic
        }
        catch (Exception ex)
        {
            Console.WriteLine("LAYOUT ERROR: " + ex);
        }

    }
}
