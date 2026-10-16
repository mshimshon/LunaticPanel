using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using StatePulse.Net;
using System.Text.Json.Nodes;
using static LunaticPanel.Engine.Keys.System.PluginKeys;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class LoadBootstrapEffect : IEffect<LoadBootstrapAction>
{
    private readonly IQueryBus _queryBus;
    private readonly ICrazyReport<DisablePluginAction> _crazyReport;

    public LoadBootstrapEffect(IQueryBus queryBus, ICrazyReport<DisablePluginAction> crazyReport)
    {
        _queryBus = queryBus;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
    }

    public async Task EffectAsync(LoadBootstrapAction action, IDispatcher dispatcher)
    {
        try
        {
            var queryResponse = await _queryBus.QueryWithoutDataAsync(Queries.FetchBootstrapFile);
            var resultObj = queryResponse.Data?.GetData();
            if (resultObj == default || resultObj.GetType() != typeof(JsonArray))
                throw new Exception("Cannot be null"); // TODO: USE REAL ERROR
            JsonArray result = (JsonArray)resultObj;
            List<string> enabled = result
                .Where(x => x?["Entity"]!["Lifecycle"]!["StartupState"]?.GetValue<int>() == 1)
                .Select(x => x!["Entity"]!["Identity"]!["PackageId"]!.GetValue<string>())
                .ToList();

            List<string> disabled = result
                .Where(x => x?["Entity"]!["Lifecycle"]!["StartupState"]?.GetValue<int>() == 0)
                .Select(x => x!["Entity"]!["Identity"]!["PackageId"]!.GetValue<string>())
                .ToList();
            await dispatcher.Prepare<LoadBootstrapDoneAction>()
                .With(p => p.Enabled, enabled)
                .With(p => p.Disabled, disabled)
                .DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<LoadBootstrapDoneAction>().DispatchAsync();

            throw;
        }

    }
}
