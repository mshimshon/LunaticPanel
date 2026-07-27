using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;
using LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

namespace LunaticPanel.Package.LocalServer.Infrastructure.LunaPackage.Exceptions;

public class PackageFileNotFoundException : InfrastructureException
{
    public PackageFileNotFoundException(PackageId id, PackageVersion version) :
        base(nameof(PackageFileNotFoundException), $"File not found for {id.Value} v{version.Value}")
    {
    }
}
