using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageAuthorLengthException : DomainCodedException
{
    public PackageAuthorLengthException() :
        base(nameof(PackageAuthorLengthException),
        $"{nameof(ManifestEntity.Author)} must be minimum {DomainValidationExt.PKG_AUTHOR_MIN_LENGTH} characters and must not exceed {DomainValidationExt.PKG_AUTHOR_MIN_LENGTH} characters"
        )

    {
    }
}
