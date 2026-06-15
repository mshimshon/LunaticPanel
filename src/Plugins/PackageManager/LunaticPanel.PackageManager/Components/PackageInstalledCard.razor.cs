using LunaticPanel.PackageManager.Application.Pulses.States.Models;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.PackageManager.Components;

public partial class PackageInstalledCard
{
    private const string PACKAGE_ENABLED = "Enabled"; // TODO: LOCALIZE
    private const string PACKAGE_DISABLED = "Disabled"; // TODO: LOCALIZE
    private const string PACKAGE_UPDATE = "Schedule Update to {0}"; // TODO: LOCALIZE
    private const string PACKAGE_UPDATE_CANCEL = "Cancel Update to {0}"; // TODO: LOCALIZE
    private const string PACKAGE_UPDATE_ROLLBACK_CONFLICT = "Cannot Update (Rollback Scheduled)"; // TODO: LOCALIZE
    private const string PACKAGE_ROLLBACK = "Schedule Rollback to {0}"; // TODO: LOCALIZE
    private const string PACKAGE_ROLLBACK_CANCEL = "Cancel Rollback to {0}"; // TODO: LOCALIZE
    private const string PACKAGE_ROLLBACK_UPDATE_CONFLICT = "Cannot Rollback (Update Scheduled)"; // TODO: LOCALIZE

    [Parameter] public PackageLocalPulseModel Data { get; set; } = default!;
    protected override void OnWidgetParametersSet()
    {
        ViewModel.DataModel = Data;
    }
}
