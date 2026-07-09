using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageIdPatternViolationException : DomainCodedException
{
    public PackageIdPatternViolationException(string id) :
        base(nameof(PackageIdPatternViolationException), $"'{id}' is not a valid pattern expected Pattern (My.Package)")
    {
    }
}
