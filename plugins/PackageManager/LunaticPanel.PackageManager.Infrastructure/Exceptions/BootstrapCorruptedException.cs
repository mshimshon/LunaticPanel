using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

internal class BootstrapCorruptedException : HostCodedException
{
    public BootstrapCorruptedException() : base(nameof(BootstrapCorruptedException), "Couldn't read bootstrap.json. Corruption maybe?")
    {
    }
}
