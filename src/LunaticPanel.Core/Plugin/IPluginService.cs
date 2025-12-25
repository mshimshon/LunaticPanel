namespace LunaticPanel.Core.Plugin;

public interface IPluginService<TPlugin>
    where TPlugin : IPlugin
{
    TService GetRequired<TService>() where TService : notnull;
}
