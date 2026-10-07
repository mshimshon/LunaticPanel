using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageEndOfLifeMessageViolationException : DomainCodedException
{
    public PackageEndOfLifeMessageViolationException() :
        base(nameof(PackageEndOfLifeMessageViolationException), $"ASCII characters only allowed in {nameof(ManifestEntity.EndOfLifeMessage)}")
    {
    }
}
