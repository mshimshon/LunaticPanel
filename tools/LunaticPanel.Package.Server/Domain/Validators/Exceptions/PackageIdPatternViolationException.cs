using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageIdPatternViolationException : DomainCodedException
{
    public PackageIdPatternViolationException(string id) :
        base(nameof(PackageIdPatternViolationException), $"'{id}' is not a valid pattern expected Pattern (My.Package)")
    {
    }
}
