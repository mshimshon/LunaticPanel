using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageDescriptionPatternViolationException : DomainCodedException
{
    public PackageDescriptionPatternViolationException() :
        base(nameof(PackageDescriptionPatternViolationException), "Package description acan only contain valid ASCII characters.")
    {
    }
}
