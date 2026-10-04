using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Keys;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageSearchCardViewModel : WidgetViewModelBase, IPackageSearchCardViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IMedihater _medihater;
    private readonly ICrazyReport<PackageSearchCardViewModel> _crazyReport;

    public PackageManagerDiskState ManagerState => _statePulse.StateOf<PackageManagerDiskState>(() => this, UpdateChanges);


    public PackageInfoPayload Data { get; set; } = default!;
    public bool IsInstalled { get; set; }
    public bool IsPendingUpdate { get; set; }
    public bool IsDeleted { get; set; }
    public PackageSearchCardViewModel(IStatePulse statePulse, IMedihater medihater, ICrazyReport<PackageSearchCardViewModel> crazyReport)
    {
        _statePulse = statePulse;
        _medihater = medihater;
        _crazyReport = crazyReport;
        _crazyReport.SetModule(LPPackageManagerKeys.MODULE_NAME);
    }

    protected override void OnViewModelBeforeRender()
    {
        IsInstalled = ManagerState.Installed.Any(p => p.Info.PackageId == Data.PackageId);
        IsPendingUpdate = ManagerState.PendingUpdates.Any(p => p.Info.PackageId == Data.PackageId);
        IsDeleted = ManagerState.PendingDelete.Any(p => p.Info.PackageId == Data.PackageId);
    }
    public async Task InstallAsync() => await FailSafeExecutionAsync(InstallProcessAsync);
    public async Task InstallProcessAsync()
    {
        // TODO: Implement two stage Download, Then Install from Cache.
        try
        {
            IsLoading = true;
            var result = await _medihater.Send(new GetPackagesLatestVersionQuery([Data.PackageId]));
            if (result.Count <= 0)
                throw new HostCodedException("NotFound", "Cannot find package.");
            var target = result.First();
            await _statePulse.Dispatcher.Prepare<InstallPackageAction>()
                .With(p => p.Target, target)
                .DispatchAsync();
        }
        catch (Exception ex)
        {
            _crazyReport.ReportErrorException(ex.Message, ex);
            throw;
        }
        finally
        {
            IsLoading = false;

        }

    }
}
