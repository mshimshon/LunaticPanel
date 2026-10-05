using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageAuthorRequiredException : DomainCodedException
{
    public PackageAuthorRequiredException() :
        base(nameof(PackageAuthorRequiredException), $"{nameof(ManifestEntity.Author)} is required.")
    {
    }
}
