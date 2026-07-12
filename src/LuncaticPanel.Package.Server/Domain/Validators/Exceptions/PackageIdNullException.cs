using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageIdNullException : DomainCodedException
{
    public PackageIdNullException() :
        base(nameof(PackageIdNullException), "Package Id cannot be null or empty.")
    {
    }
}
