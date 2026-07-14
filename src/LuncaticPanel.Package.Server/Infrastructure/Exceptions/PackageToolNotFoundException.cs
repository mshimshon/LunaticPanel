namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageToolNotFoundException : InfrastructureCodedException
{
    public PackageToolNotFoundException(string panelVersion) :
        base(nameof(PackageToolNotFoundException), $"No tool released for major version {panelVersion} on github.")
    {
    }
}
