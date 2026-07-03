using System.Reflection;

namespace LunaticPanel.Engine.Plugin;

public interface IPluginLoader
{

    Assembly Load();
    void Unload();
}
