using LuncaticPanel.Package.Server.Domain.Exceptions;
using LuncaticPanel.Package.Server.Domain.Validators;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageTitleLengthException : DomainCodedException
{
    public PackageTitleLengthException() :
        base(nameof(PackageTitleLengthException),
        $"Package Title must be minimum {DomainValidationExt.PKG_TITLE_MIN_LENGTH} characters and must not exceed {DomainValidationExt.PKG_TITLE_MAX_LENGTH} characters")
    {
    }
}
