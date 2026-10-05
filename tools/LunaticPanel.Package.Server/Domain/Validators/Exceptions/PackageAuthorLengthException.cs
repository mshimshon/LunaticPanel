using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageAuthorLengthException : DomainCodedException
{
    public PackageAuthorLengthException() :
        base(nameof(PackageAuthorLengthException),
        $"{nameof(ManifestEntity.Author)} must be minimum {DomainValidationExt.PKG_AUTHOR_MIN_LENGTH} characters and must not exceed {DomainValidationExt.PKG_AUTHOR_MIN_LENGTH} characters"
        )

    {
    }
}
