using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.Exceptions;

namespace LunaticPanel.PackageManager.Domain.Entities.Exceptions;

public sealed class PackageVersionInvalidException : DomainException
{
    public PackageVersionInvalidException(string version) :
        base(nameof(PackageVersionInvalidException), $"{nameof(PackageVersion)}{version} not valid format should be as such '1.1.1' major.minor.patch")
    {
    }
}
