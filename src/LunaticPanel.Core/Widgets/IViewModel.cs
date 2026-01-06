namespace LunaticPanel.Core.Widgets;

public interface IViewModel
{
    public event Func<Task>? SpreadChanges;
    public bool IsLoading { get; }
}
