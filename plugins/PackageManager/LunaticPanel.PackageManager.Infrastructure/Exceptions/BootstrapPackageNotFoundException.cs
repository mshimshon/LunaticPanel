using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

public class BootstrapPackageNotFoundException : HostCodedException
{
    public BootstrapPackageNotFoundException(string packageId) :
        base(nameof(BootstrapPackageNotFoundException), $"{packageId} not found.")
    {
    }
}
