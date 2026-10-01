using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageDescriptionPatternViolationException : DomainCodedException
{
    public PackageDescriptionPatternViolationException() :
        base(nameof(PackageDescriptionPatternViolationException), "Package description acan only contain valid ASCII characters.")
    {
    }
}
