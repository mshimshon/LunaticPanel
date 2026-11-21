using LunaticPanel.Core.Abstraction.Messaging.QuerySystem;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Pulses.Actions;
using LunaticPanel.PackageManager.Keys;
using StatePulse.Net;
using System.Text.Json.Nodes;
using static LunaticPanel.Engine.Keys.System.PluginKeys;
using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Pulses.Effects;

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
            var queryResponse = await _queryBus.QueryWithoutDataAsync(Queries.FETCH_BOOTSTRAP_FILE);
            var resultObj = queryResponse.Data?.GetData();
            if (resultObj == default || resultObj.GetType() != typeof(JsonArray))
                throw new Exception("Cannot be null"); // TODO: USE REAL ERROR
            var result = (JsonArray)resultObj;
            var enabled = result
                .Where(x => x?["StartupState"]?.GetValue<int>() == 1)
                .Select(x => x!["Manifest"]!["Id"]!.GetValue<string>())
                .ToList();

            var disabled = result
                .Where(x => x?["StartupState"]?.GetValue<int>() == 0)
                .Select(x => x!["Manifest"]!["Id"]!.GetValue<string>())
                .ToList();
            await dispatcher.Prepare<LoadBootstrapDoneAction>()
                .With(p => p.Enabled, enabled)
                .With(p => p.Disabled, disabled)
                .DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<LoadBootstrapDoneAction>().DispatchAsync();

            throw new HostUnkownException();
        }

    }
}
