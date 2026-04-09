namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Extensions;

public abstract class LinuxRegionContext
{
    public ILinuxCommand LinuxCommand { get; set; } = default!;

    public T CreateFrom<T>(LinuxRegionContext context) where T : LinuxRegionContext, new()
    => Create<T>(context.LinuxCommand);

    public static T Create<T>(ILinuxCommand command)
        where T : LinuxRegionContext, new()
    {
        var instance = new T();
        RegionFactory<T>.AssignLinuxCommand(instance, command);
        return instance;
    }
}