using LunaticPanel.Package.Server.Domain.Entites;
using LunaticPanel.Package.Server.Domain.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Validators.Exceptions;

public sealed class PackageEndOfLifeMessageEmptyException : DomainCodedException
{
    public PackageEndOfLifeMessageEmptyException() :
        base(nameof(PackageEndOfLifeMessageEmptyException), $"{nameof(ManifestEntity.EndOfLifeMessage)} cannot be null or empty when set.")
    {
    }
}
