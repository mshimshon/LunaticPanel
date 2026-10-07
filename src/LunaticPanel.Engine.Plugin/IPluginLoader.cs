using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public interface IPluginLoader
{
    bool IsLoaded { get; }
    Assembly Load();
    void Unload();
}
