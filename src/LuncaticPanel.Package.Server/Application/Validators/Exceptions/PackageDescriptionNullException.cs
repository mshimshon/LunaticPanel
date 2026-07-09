using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageDescriptionNullException : DomainCodedException
{
    public PackageDescriptionNullException() :
        base(nameof(PackageDescriptionNullException), "Package description cannot be null or empty.")
    {
    }
}
