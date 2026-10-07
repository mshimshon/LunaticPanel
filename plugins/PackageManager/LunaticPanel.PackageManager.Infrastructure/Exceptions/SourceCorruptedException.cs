using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

public class SourceCorruptedException : HostCodedException
{
    public SourceCorruptedException() :
        base(nameof(SourceCorruptedException), "Package Manager source config file seems corrupted.")
    {
    }
}
