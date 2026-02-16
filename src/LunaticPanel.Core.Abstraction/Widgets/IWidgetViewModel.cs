using LunaticPanel.Core.Abstraction.Widgets.Enum;

namespace LunaticPanel.Core.Abstraction.Widgets;

public interface IWidgetViewModel
{
    public event Func<SpreadChangeOption, Task>? SpreadChanges;
    public bool IsLoading { get; }
}
