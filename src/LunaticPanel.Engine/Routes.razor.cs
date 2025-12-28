using System.Reflection;

namespace LunaticPanel.Engine;

public partial class Routes
{
    public static List<Assembly> AdditionalAssemblies { get; set; } = new List<Assembly>();

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender) InvokeAsync(StateHasChanged);
    }
}
