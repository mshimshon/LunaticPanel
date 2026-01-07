namespace LunaticPanel.Core.Abstraction;

public interface IPluginContextService : IPluginContext
{
    TService GetRequired<TService>() where TService : notnull;

}