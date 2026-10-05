using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;
using LunaticPanel.Package.Server.Domain.Validators;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageDescriptionLengthException : DomainCodedException
{
    public PackageDescriptionLengthException() :
        base(nameof(PackageDescriptionLengthException),
        $"{nameof(ManifestEntity.Description)} must be minimum {DomainValidationExt.PKG_DESC_MIN_LENGTH} characters and must not exceed {DomainValidationExt.PKG_DESC_MAX_LENGTH} characters")
    {
    }
}
