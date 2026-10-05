using LunaticPanel.Package.Server.Application.Exceptions;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands.Exceptions;

public sealed class PackageVersionExistException : AppLayerException
{
    public PackageVersionExistException(string id, string version) :
        base(nameof(PackageVersionExistException), $"{id} v{version} already exist.")
    {
    }
}
