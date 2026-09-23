using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.Actions;
using LunaticPanel.PackageManager.Application.Pulses.States;
using LunaticPanel.PackageManager.Application.Services;
using MedihatR;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageSearchCardViewModel : WidgetViewModelBase, IPackageSearchCardViewModel
{
    private readonly IStatePulse _statePulse;
    private readonly IMedihater _medihater;
    private readonly IRepositorySourceService _repositorySourceService;

    public PackageManagerDiskState ManagerState => _statePulse.StateOf<PackageManagerDiskState>(() => this, UpdateChanges);


    public PackageInfoPayload Data { get; set; } = default!;
    public bool IsInstalled { get; set; }
    public PackageSearchCardViewModel(IStatePulse statePulse, IMedihater medihater)
    {
        _statePulse = statePulse;
        _medihater = medihater;
    }

    protected override void OnViewModelBeforeRender()
    {
        IsInstalled = ManagerState.Installed.Any(p => p.Info.PackageId == Data.PackageId);
    }
    /*
     BIN/lunaticpanel/plugins -> Folder of active plugins on panel.
        BIN/lunaticpanel/plugins_preinstalled -> Always install + enabled if not present in plugins
        TMP/lunaticpanel/plugins/installed -> Save LPKG for the currently installed plugin, if LPKG missing or multiple version, do not load the plugin.
        TMP/lunaticpanel/plugins/apply -> Any LPKG in the folder will be installed at startup time of the panel then move to install, if multiple version do not process.
        TMP/lunaticpanel/plugins/rollbacks -> When apply occurs, if plugin was there and its a upgrade/downgrade the rollback is current lpkg

        This design allows to fix runtime lock on plugin folders and allows the host panel itself to apply updates, new install and the package manage must only cycle the files within those folders and the panel do the rest at startup.

     */
    public async Task InstallAsync()
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
        catch (Exception)
        {

            throw;
        }
        finally
        {
            IsLoading = false;

        }

    }
}
