using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class PluginCompanyNotFoundException : HostCodedException
{
    public PluginCompanyNotFoundException() :
        base(nameof(PluginCompanyNotFoundException), "The Company must be defined in your csproj it will be reflect as the author.")
    {
    }
}
