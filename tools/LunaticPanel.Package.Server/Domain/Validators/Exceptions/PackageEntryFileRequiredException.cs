using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageEntryFileRequiredException : DomainCodedException
{
    public PackageEntryFileRequiredException() :
        base(nameof(PackageEntryFileRequiredException), $"{nameof(ManifestEntity.PluginEntryFile)} is required.")
    {
    }
}
