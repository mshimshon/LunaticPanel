using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Exceptions;

public sealed class PackageVersionNotFoundException : InfrastructureException
{
    public PackageVersionNotFoundException(string id, string version) :
    base(nameof(PackageVersionNotFoundException), $"Package '{id} v{version}'  was not found.")
    {
    }
}
