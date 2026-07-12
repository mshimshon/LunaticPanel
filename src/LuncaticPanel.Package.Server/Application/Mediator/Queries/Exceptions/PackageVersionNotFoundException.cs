using LuncaticPanel.Package.Server.Application.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Exceptions;

public sealed class PackageVersionNotFoundException : AppLayerException
{
    public PackageVersionNotFoundException(string id, string version) :
        base(nameof(PackageVersionNotFoundException), $"{id} v{version} was not found.")
    {
    }
}
