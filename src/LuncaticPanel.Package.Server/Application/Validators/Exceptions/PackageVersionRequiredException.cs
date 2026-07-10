using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageVersionRequiredException : DomainCodedException
{
    public PackageVersionRequiredException() :
        base(nameof(PackageVersionRequiredException), $"the package version is required.")
    {
    }
}
