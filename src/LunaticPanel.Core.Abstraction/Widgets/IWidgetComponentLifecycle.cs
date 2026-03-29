namespace LunaticPanel.Core.Abstraction.Widgets;

public interface IWidgetComponentLifecycle
{
    Task BringComponentAlive();
    Task KillComponent();
}
