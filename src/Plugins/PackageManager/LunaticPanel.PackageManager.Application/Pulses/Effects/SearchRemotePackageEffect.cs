using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using MedihatR;
using StatePulse.Net;
using System.Text.RegularExpressions;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class SearchRemotePackageEffect : IEffect<SearchRemotePackageAction>
{
    private readonly IMedihater _medihater;

    public SearchRemotePackageEffect(IMedihater medihater)
    {
        _medihater = medihater;
    }
    private readonly string _regexValidation = @"/^[a-zA-Z0-9 .]*$/";
    public async Task EffectAsync(SearchRemotePackageAction action, IDispatcher dispatcher)
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
        var packages = await _medihater.Send(data, dispatcher.CancelToken);
        if (dispatcher.IsCancellationRequested) return;
        await dispatcher.Prepare<SearchRemotePackageDoneAction>()
            .With(p => p.Result, packages)
            .DispatchAsync(dispatcher.CancelToken);
    }
}
