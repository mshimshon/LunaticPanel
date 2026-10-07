using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageAuthorViolationException : DomainCodedException
{
    public PackageAuthorViolationException(string value) :
        base(nameof(PackageAuthorViolationException), $"'{value}' should only have a-Z 0-9.")
    {
    }
}
