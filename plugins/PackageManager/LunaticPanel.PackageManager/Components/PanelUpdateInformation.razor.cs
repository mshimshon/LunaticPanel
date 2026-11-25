using LunaticPanel.Core.Abstraction.Widgets;
using LunaticPanel.PackageManager.Components.ViewModels;
using System;

namespace LunaticPanel.PackageManager.Components;

public partial class PanelUpdateInformation : WidgetComponentBase<PluginEntry, IPanelUpdateInformationViewModel>
{
    public const string FOUND_UPDATE = "The system found a new panel update v{0}.";
    public const string PANEL_DOWNLOADED = "New panel v{0} was downloaded, waiting for compatible plugins.";
    public const string FOUND_STAGE = "System has ready upgrade v{0}, restart required.";
}
