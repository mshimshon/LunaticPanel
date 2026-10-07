using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LunaticPanel.Engine.Web.Extensions;

internal static class RenderFragmentExt
{
    public static RenderFragment CreateRenderFragmentComponent(this Type componentType, Action<RenderTreeBuilder> setParameters)
=> builder =>
{
    builder.OpenComponent(0, componentType);
    setParameters(builder);
    builder.CloseComponent();
};
}
