using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageDotnetVersionRequiredException : DomainCodedException
{
    public PackageDotnetVersionRequiredException() :
        base(nameof(PackageDotnetVersionRequiredException), $"{nameof(ManifestEntity.DotnetVersion)} is a required.")
    {
    }
}
