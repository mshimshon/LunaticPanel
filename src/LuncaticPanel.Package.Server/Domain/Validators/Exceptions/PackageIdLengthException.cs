using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageIdLengthException : DomainCodedException
{
    public PackageIdLengthException() :
        base(nameof(PackageIdLengthException),
        $"Package Id must be minimum {DomainValidationExt.PKG_ID_MIN_LENGTH} characters and must not exceed {DomainValidationExt.PKG_ID_MAX_LENGTH} characters")
    {
    }
}
