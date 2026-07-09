using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageDescriptionLengthException : DomainCodedException
{
    public PackageDescriptionLengthException() :
        base(nameof(PackageDescriptionLengthException),
        $"{nameof(ManifestEntity.Description)} must be minimum {DomainValidationExt.PKG_DESC_MIN_LENGTH} characters and must not exceed {DomainValidationExt.PKG_DESC_MAX_LENGTH} characters")
    {
    }
}
