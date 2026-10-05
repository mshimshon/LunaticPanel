using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageIdNullException : DomainCodedException
{
    public PackageIdNullException() :
        base(nameof(PackageIdNullException), "Package Id cannot be null or empty.")
    {
    }
}
