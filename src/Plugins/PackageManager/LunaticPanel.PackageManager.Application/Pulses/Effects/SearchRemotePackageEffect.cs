using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class SearchRemotePackageEffect : IEffect<SearchRemotePackageAction>
{
    private readonly IMedihater _medihater;

    public SearchRemotePackageEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }

    public async Task EffectAsync(SearchRemotePackageAction action, IDispatcher dispatcher)
    {
        var data = new SearchRepositoryQuery()
        {
            Search = new() { Keywords = action.Keywords },
            Sources = action.Sources
        };
        var packages = await _medihater.Send(data, dispatcher.CancelToken);
        await dispatcher.Prepare<SearchRemotePackageDoneAction>()
            .With(p => p.Result, packages)
            .DispatchAsync(dispatcher.CancelToken);
    }
}
