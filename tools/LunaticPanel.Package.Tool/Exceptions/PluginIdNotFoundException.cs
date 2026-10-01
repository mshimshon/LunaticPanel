using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginIdNotFoundException : HostCodedException
{
    public PluginIdNotFoundException(string file) : base(nameof(PluginIdNotFoundException), $"Plugin ID for {file} was not found.")
    {
    }
}
