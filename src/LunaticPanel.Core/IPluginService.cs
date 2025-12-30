namespace LunaticPanel.Core;

public interface IPluginService : IServiceProvider
{
    TService GetRequired<TService>() where TService : notnull;

}

public interface IPluginService<TPlugin> : IPluginService
    where TPlugin : IPlugin
{

}
