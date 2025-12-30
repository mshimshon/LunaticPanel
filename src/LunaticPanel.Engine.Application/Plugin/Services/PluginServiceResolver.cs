using LunaticPanel.Core;
using LunaticPanel.Core.Plugin;
using LunaticPanel.Engine.Application.Plugin.Services.DependencyController;

namespace LunaticPanel.Engine.Application.Plugin.Services;

public class PluginServiceResolver<TPlugin> : IPluginService<TPlugin>
    where TPlugin : IPlugin

{
    private readonly PluginDependencyInjectionController _pluginDependencyInjectionController;

    public PluginServiceResolver(PluginDependencyInjectionController pluginDependencyInjectionController)
    {
        _pluginDependencyInjectionController = pluginDependencyInjectionController;
        _pluginDependencyInjectionController.Configure(typeof(TPlugin));
    }

    public TService GetRequired<TService>() where TService : notnull => (TService)GetService(typeof(TService));
    public object GetService(Type serviceType) => _pluginDependencyInjectionController.GetRequiredService(typeof(TPlugin), serviceType, 0);
}
