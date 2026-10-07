using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageDotnetVersionRequiredException : DomainCodedException
{
    public PackageDotnetVersionRequiredException() :
        base(nameof(PackageDotnetVersionRequiredException), $"{nameof(ManifestEntity.DotnetVersion)} is a required.")
    {
    }
}
