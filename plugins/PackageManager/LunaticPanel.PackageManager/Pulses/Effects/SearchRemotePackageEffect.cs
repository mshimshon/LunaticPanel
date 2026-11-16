using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;
using System.Text.RegularExpressions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

internal class SearchRemotePackageEffect : IEffect<SearchRemotePackageAction>
{
    private readonly IMediator _mediator;

    public SearchRemotePackageEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task EffectAsync(SearchRemotePackageAction action, IDispatcher dispatcher)
    {
        try
        {
            var data = new SearchRepositoryQuery()
            {
                Search = new()
                {
                    Keywords = action.Keywords,
                },
                Sources = action.Sources
            };
            if (dispatcher.IsCancellationRequested) return;
            var packages = await _mediator.ExecuteAsync(data, dispatcher.CancelToken);
            if (dispatcher.IsCancellationRequested) return;
            await dispatcher.Prepare<SearchRemotePackageDoneAction>()
                .With(p => p.Result, packages)
                .DispatchAsync(dispatcher.CancelToken);
        }
        catch
        {
            await Task.Delay(1000);
            await dispatcher.Prepare<SearchRemotePackageDoneAction>().DispatchAsync(dispatcher.CancelToken);
            throw new HostUnkownException();
        }

    }
}
