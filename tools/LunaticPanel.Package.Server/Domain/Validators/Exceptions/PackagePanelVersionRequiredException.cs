using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackagePanelVersionRequiredException : DomainCodedException
{
    public PackagePanelVersionRequiredException() :
        base(nameof(PackagePanelVersionRequiredException), $"{nameof(ManifestEntity.PanelVersion)} is a required.")
    {
    }
}
