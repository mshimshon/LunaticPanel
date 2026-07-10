using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageEntryFileRequiredException : DomainCodedException
{
    public PackageEntryFileRequiredException() :
        base(nameof(PackageEntryFileRequiredException), $"{nameof(ManifestEntity.PluginEntryFile)} is required.")
    {
    }
}
