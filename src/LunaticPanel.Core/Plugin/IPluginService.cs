namespace LunaticPanel.Core.Plugin;

public interface IPluginService
{
    TService GetRequired<TService>() where TService : notnull;

}
public interface IPluginService<TPlugin> : IPluginService
    where TPlugin : IPlugin
{
}
