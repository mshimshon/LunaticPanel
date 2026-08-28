using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Pulses.States;
using StatePulse.Net;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageSearchCardViewModel : WidgetViewModelBase, IPackageSearchCardViewModel
{
    private readonly IStatePulse _statePulse;

    public PackageManagerState ManagerState => _statePulse.StateOf<PackageManagerState>(() => this, UpdateChanges);


    public PackageInfoPayload Data { get; set; } = default!;
    public bool IsInstalled { get; set; }
    public PackageSearchCardViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }

    protected override void OnViewModelBeforeRender()
    {
        IsInstalled = ManagerState.InstalledPackages.Any(p => p.Info.PackageId == Data.PackageId);
    }
    /*
     BIN/lunaticpanel/plugins -> Folder of active plugins on panel.
        BIN/lunaticpanel/plugins_preinstalled -> Always install + enabled if not present in plugins
        TMP/lunaticpanel/plugins/installed -> Save LPKG for the currently installed plugin, if LPKG missing or multiple version, do not load the plugin.
        TMP/lunaticpanel/plugins/apply -> Any LPKG in the folder will be installed at startup time of the panel then move to install, if multiple version do not process.
        TMP/lunaticpanel/plugins/rollbacks -> When apply occurs, if plugin was there and its a upgrade/downgrade the rollback is current lpkg

        This design allows to fix runtime lock on plugin folders and allows the host panel itself to apply updates, new install and the package manage must only cycle the files within those folders and the panel do the rest at startup.

     */
    public Task InstallAsync() => throw new NotImplementedException();
}
