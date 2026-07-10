using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackagePanelVersionRequiredException : DomainCodedException
{
    public PackagePanelVersionRequiredException() :
        base(nameof(PackagePanelVersionRequiredException), $"{nameof(ManifestEntity.PanelVersion)} is a required.")
    {
    }
}
