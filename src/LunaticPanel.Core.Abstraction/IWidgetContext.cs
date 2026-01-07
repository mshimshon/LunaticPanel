using LunaticPanel.Core.Abstraction.Widgets;

namespace LunaticPanel.Core.Abstraction;

public interface IWidgetContext : IPluginContext
{
    TWidgetViewModel GetViewModel<TWidgetViewModel>() where TWidgetViewModel : IWidgetViewModel;

}
