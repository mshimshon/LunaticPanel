namespace LunaticPanel.Core.Abstraction.Widgets;

public interface IWidgetViewModel
{
    public event Func<Task>? SpreadChanges;
    public bool IsLoading { get; }
}
