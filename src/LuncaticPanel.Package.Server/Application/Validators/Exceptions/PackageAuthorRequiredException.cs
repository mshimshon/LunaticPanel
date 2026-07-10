using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageAuthorRequiredException : DomainCodedException
{
    public PackageAuthorRequiredException() :
        base(nameof(PackageAuthorRequiredException), $"{nameof(ManifestEntity.Author)} is required.")
    {
    }
}
