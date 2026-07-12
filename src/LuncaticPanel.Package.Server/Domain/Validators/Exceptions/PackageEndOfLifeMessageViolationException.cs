using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageEndOfLifeMessageViolationException : DomainCodedException
{
    public PackageEndOfLifeMessageViolationException() :
        base(nameof(PackageEndOfLifeMessageViolationException), $"ASCII characters only allowed in {nameof(ManifestEntity.EndOfLifeMessage)}")
    {
    }
}
