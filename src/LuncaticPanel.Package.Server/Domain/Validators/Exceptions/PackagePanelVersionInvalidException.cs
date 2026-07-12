using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackagePanelVersionInvalidException : DomainCodedException
{
    public PackagePanelVersionInvalidException(string panelVersion) :
        base(nameof(PackagePanelVersionInvalidException), $"'{panelVersion}' {nameof(ManifestEntity.PanelVersion)} is not valid.")
    {
    }
}
