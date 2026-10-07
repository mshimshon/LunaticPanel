using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageDescriptionNullException : DomainCodedException
{
    public PackageDescriptionNullException() :
        base(nameof(PackageDescriptionNullException), "Package description cannot be null or empty.")
    {
    }
}
