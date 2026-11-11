using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using MaksimShimshon.Mediator;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Application.Pulses.Effects;

internal class PackageRollbackEffect : IEffect<PackageRollbackAction>
{
    private readonly IMediator _mediator;
    private readonly IStateAccessor<PackageManagerDiskEditorState> _packageManagerDiskEditorState;
    private readonly ICrazyReport<PackageRollbackEffect> _crazyReport;

    public PackageRollbackEffect(IMediator mediator, IStateAccessor<PackageManagerDiskEditorState> packageManagerDiskEditorState, ICrazyReport<PackageRollbackEffect> crazyReport)
    {
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
        _crazyReport.Report("Initialized Class");
        _mediator = mediator;
        _packageManagerDiskEditorState = packageManagerDiskEditorState;
    }

    public async Task EffectAsync(PackageRollbackAction action, IDispatcher dispatcher)
    {
        try
        {
            _crazyReport.Report("Executing Effect");
            await _mediator.ExecuteAsync(new PackageRollbackCommand(action.Package), dispatcher.CancelToken);
            var newPendingRollbacks = _packageManagerDiskEditorState.State.PendingRollbacks.ToList();
            newPendingRollbacks.Add(action.Package);
            await dispatcher.Prepare<PackageRollbackDoneAction>()
                .With(p => p.Packages, newPendingRollbacks)
                .DispatchAsync();
        }
        catch (Exception ex)
        {
            await dispatcher.Prepare<PackageRollbackDoneAction>().DispatchAsync();
            throw;
        }
    }
}
