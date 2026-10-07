using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

public class InstallNotFoundException : HostCodedException
{
    public InstallNotFoundException(string packageId) :
        base(nameof(InstallNotFoundException), $"{packageId} not found.")
    {
    }
}
