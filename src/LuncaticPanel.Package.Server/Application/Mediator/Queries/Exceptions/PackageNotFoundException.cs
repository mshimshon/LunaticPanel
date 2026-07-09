using LuncaticPanel.Package.Server.Application.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Exceptions;

public class PackageNotFoundException : AppLayerException
{
    public PackageNotFoundException(string id, string version) :
        base(nameof(PackageNotFoundException), $"'{id}' v{version} was not found.")
    {
    }

    public PackageNotFoundException(string id) :
    base(nameof(PackageNotFoundException), $"'{id}' was not found.")
    {
    }
}
