using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

public class PackageNotFoundException : HostCodedException
{
    public PackageNotFoundException(string id, string version) :
        base(nameof(PackageNotFoundException), $"{id} v{version} could not be found in sources.")
    {
    }
}
