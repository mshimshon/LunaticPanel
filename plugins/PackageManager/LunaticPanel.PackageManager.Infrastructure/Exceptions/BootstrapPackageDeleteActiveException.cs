using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

public class BootstrapPackageDeleteActiveException : HostCodedException
{
    public BootstrapPackageDeleteActiveException(string packageId) :
        base(nameof(BootstrapPackageDeleteActiveException), $"{packageId} is active therefore cannot be delete, disable the plugin and reboot panel before deleting")
    {
    }
}
