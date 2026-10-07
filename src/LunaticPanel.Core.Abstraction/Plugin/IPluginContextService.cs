namespace LunaticPanel.Core.Abstraction.Plugin;

public interface IPluginContextService : IPluginContext
{
    TService GetRequired<TService>() where TService : notnull;

}