using LuncaticPanel.Package.Server.Domain.Entites;
using LuncaticPanel.Package.Server.Domain.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Validators.Exceptions;

public sealed class PackageEndOfLifeMessageEmptyException : DomainCodedException
{
    public PackageEndOfLifeMessageEmptyException() :
        base(nameof(PackageEndOfLifeMessageEmptyException), $"{nameof(ManifestEntity.EndOfLifeMessage)} cannot be null or empty when set.")
    {
    }
}
