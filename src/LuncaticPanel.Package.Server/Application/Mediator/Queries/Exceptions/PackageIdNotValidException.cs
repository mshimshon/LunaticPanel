using LuncaticPanel.Package.Server.Application.Exceptions;

namespace LuncaticPanel.Package.Server.Application.Mediator.Queries.Exceptions;

public class PackageIdNotValidException : AppLayerException
{
    public PackageIdNotValidException(string id) :
        base(nameof(PackageIdNotValidException), $"{id} is not a valid package id.")
    {
    }
}
