using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.Exceptions;

namespace LunaticPanel.PackageManager.Domain.Entities.Exceptions;

public sealed class PackageVersionInvalidException : DomainException
{
    public PackageVersionInvalidException(string version) :
        base(nameof(PackageVersionInvalidException), $"{nameof(PackageVersion)}{version} not valid format. SemVer supported are '1.1.1' and '1.1.1-WORD.1'")
    {
    }
}
