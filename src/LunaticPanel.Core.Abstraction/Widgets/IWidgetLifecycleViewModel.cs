namespace LunaticPanel.Core.Abstraction.Widgets;

internal interface IWidgetLifecycleViewModel
{
    void OnInitialized();
    Task OnInitializedAsync();
    void OnParametersSet();
    Task OnParametersSetAsync();
    Task OnAfterRenderAsync(bool firstRender);
    void OnAfterRender(bool firstRender);
}
