namespace LunaticPanel.Core.Abstraction.Widgets;

internal interface IWidgetLifecycleViewModel
{
    void OnInitialized();
    Task OnInitializedAsync();
    void OnParametersSet();
    Task OnParametersSetAsync();
    void OnBeforeRender();
    Task OnBeforeRenderAsync();
    Task OnAfterRenderAsync(bool firstRender);
    void OnAfterRender(bool firstRender);
}
