using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.Exceptions;

namespace LunaticPanel.PackageManager.Domain.Respositories.Exceptions;

public class PackageNotFoundException : DomainException
{
    public PackageNotFoundException(PackageId packageId)
        : base(nameof(PackageNotFoundException), $"{packageId} was not found")
    {
    }
}
