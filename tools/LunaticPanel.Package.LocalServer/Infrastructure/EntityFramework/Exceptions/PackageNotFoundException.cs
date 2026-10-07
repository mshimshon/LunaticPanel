using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Exceptions;

public sealed class PackageNotFoundException : InfrastructureException
{
    public PackageNotFoundException(string id) :
        base(nameof(PackageNotFoundException), $"Package '{id}' was not found.")
    {
    }
}
