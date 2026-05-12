using LunaticPanel.Core.Abstraction.Plugin;

namespace LunaticPanel.Core.Abstraction.Widgets;

public interface IWidgetContext : IPluginContext
{
    TWidgetViewModel GetViewModel<TWidgetViewModel>() where TWidgetViewModel : IWidgetViewModel;

}
