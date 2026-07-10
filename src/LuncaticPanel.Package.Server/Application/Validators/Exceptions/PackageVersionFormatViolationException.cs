using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageVersionFormatViolationException : DomainCodedException
{
    public PackageVersionFormatViolationException(string version) :
        base(nameof(PackageVersionFormatViolationException), $"'{version} violates the required format x.x.x")
    {
    }
}
