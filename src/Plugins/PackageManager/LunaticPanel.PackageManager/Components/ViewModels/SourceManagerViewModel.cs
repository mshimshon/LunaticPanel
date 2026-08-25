using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using MudBlazor;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class SourceManagerViewModel : WidgetViewModelBase, ISourceManagerViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IDialogService _dialogService;

    public RepositorySourceState SourceState => _statePulse.StateOf<RepositorySourceState>(() => this, UpdateChanges);
    public SourceManagerViewModel(IStatePulse statePulse, IDialogService dialogService)
    {
        _statePulse = statePulse;
        _dialogService = dialogService;
    }
    public async Task LoadSources()
    {
        await _statePulse.Dispatcher.Prepare<LoadSourcesAction>().DispatchAsync();
    }

    public Task TestSources() => throw new NotImplementedException();
    public async Task AddSource()
    {
        DialogOptions options = new()
        {
            BackdropClick = false,
            CloseButton = true,
            CloseOnEscapeKey = false,
            CloseOnNavigation = true,
            Position = DialogPosition.Center,
            NoHeader = true
        };
        IDialogReference exitRef = await _dialogService.ShowAsync<SourceAddFormDialog>("Add Source", options);
        var result = await exitRef.GetReturnValueAsync<RepositorySourcePayload>();
        if (result == default) return;
        IsLoading = true;
        var save = SourceState.Sources.ToList();
        save.Add(result);
        await _statePulse.Dispatcher.Prepare<SaveSourcesAction>()
            .With(p => p.Sources, save)
            .DispatchAsync();
        IsLoading = false;
    }
}
