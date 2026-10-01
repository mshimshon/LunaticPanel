using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class PackageRollbackEffect : IEffect<PackageRollbackAction>
{
    private readonly IMedihater _medihater;
    private readonly IStateAccessor<PackageManagerDiskEditorState> _packageManagerDiskEditorState;
    private readonly ICrazyReport<PackageRollbackEffect> _crazyReport;

    public PackageRollbackEffect(IMedihater medihater, IStateAccessor<PackageManagerDiskEditorState> packageManagerDiskEditorState, ICrazyReport<PackageRollbackEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _medihater = medihater;
        _packageManagerDiskEditorState = packageManagerDiskEditorState;
    }

    public async Task EffectAsync(PackageRollbackAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _medihater.Send(new PackageRollbackCommand(action.Package), dispatcher.CancelToken);
            var newPendingRollbacks = _packageManagerDiskEditorState.State.PendingRollbacks.ToList();
            newPendingRollbacks.Add(action.Package);
            await dispatcher.Prepare<PackageRollbackDoneAction>()
                .With(p => p.Packages, newPendingRollbacks)
                .DispatchAsync();
        }
        catch (Exception)
        {
            await dispatcher.Prepare<PackageRollbackDoneAction>().DispatchAsync();
            throw;
        }
    }
}
