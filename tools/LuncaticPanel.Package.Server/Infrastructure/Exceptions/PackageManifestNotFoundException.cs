namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageManifestNotFoundException : InfrastructureCodedException
{
    public PackageManifestNotFoundException() :
        base(nameof(PackageManifestNotFoundException), "manifest.json not found in lpkg.")
    {
    }
}
