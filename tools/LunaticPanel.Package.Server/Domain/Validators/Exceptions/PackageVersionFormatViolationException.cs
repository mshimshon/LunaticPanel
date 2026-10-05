using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageVersionFormatViolationException : DomainCodedException
{
    public PackageVersionFormatViolationException(string version) :
        base(nameof(PackageVersionFormatViolationException), $"'{version} violates the required format x.x.x")
    {
    }
}
