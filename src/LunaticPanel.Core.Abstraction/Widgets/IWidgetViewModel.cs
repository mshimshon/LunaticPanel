using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Widgets.Enum;

namespace LunaticPanel.Core.Abstraction.Widgets;

public interface IWidgetViewModel
{
    event Func<SpreadChangeOption, Task>? SpreadChanges;
    bool IsLoading { get; }
    bool FirstRenderCompleted { get; }
    void SetHostExceptionHandler(IHostExceptionHandler exceptionHandler);

}
