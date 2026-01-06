using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.Widgets;

[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "SonarLint",
    "S3881:Implement the IDisposable pattern correctly",
    Justification = "Blazor components do not use the full dispose pattern.")]
public abstract class WidgetComponentBase<TPluginEntry, TViewModel> : ComponentBase, IDisposable
    where TViewModel : IViewModel
    where TPluginEntry : IPlugin
{
    protected TViewModel ViewModel { get; private set; } = default!;
    [Inject] protected IPluginService<TPluginEntry> PluginService { get; set; } = default!;

    public Task OnViewModelChanged() => InvokeAsync(StateHasChanged);
    protected override void OnInitialized()
    {
        ViewModel = PluginService.GetRequired<TViewModel>();
        ViewModel.SpreadChanges += OnViewModelChanged;
        ThenInitialized();
    }
    protected virtual void ThenInitialized()
    {

    }
    public void Dispose()
    {
        if (ViewModel is not null)
            ViewModel.SpreadChanges -= OnViewModelChanged;
    }
}
