using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackagePanelVersionInvalidException : DomainCodedException
{
    public PackagePanelVersionInvalidException(string panelVersion) :
        base(nameof(PackagePanelVersionInvalidException), $"'{panelVersion}' {nameof(ManifestEntity.PanelVersion)} is not valid.")
    {
    }
}
