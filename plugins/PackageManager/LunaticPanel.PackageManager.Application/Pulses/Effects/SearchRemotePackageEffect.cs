using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MaksimShimshon.Mediator;
using StatePulse.Net;
using System.Text.RegularExpressions;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class SearchRemotePackageEffect : IEffect<SearchRemotePackageAction>
{
    private readonly IMediator _mediator;

    public SearchRemotePackageEffect(IMediator mediator)
    {
        _mediator = mediator;
    }
    private readonly string _regexValidation = @"/^[a-zA-Z0-9 .]*$/";
    public async Task EffectAsync(SearchRemotePackageAction action, IDispatcher dispatcher)
    {
        try
        {
            var keywordsClean = action.Keywords.Where(p => Regex.IsMatch($"{p}", _regexValidation, RegexOptions.IgnoreCase));
            var data = new SearchRepositoryQuery()
            {
                Search = new()
                {
                    Keywords = string.Join("", keywordsClean),
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
            throw;
        }

    }
}
