using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Application.Pulses.States.Models;

namespace LunaticPanel.PackageManager.Components.ViewModels;

internal class PackageInstalledCardViewModel : WidgetViewModelBase, IPackageInstalledCardViewModel
{
    public PackageLocalPulseModel DataModel { get; set; } = default!;

    public bool HasUpdateAvailable => DataModel.Update != default && DataModel.Update.Version != DataModel.Package.Version;

    public bool CheckingForUpdate => DataModel.IsUpdateLoading;

    protected override void OnViewModelParametersSet()
    {

    }
}
