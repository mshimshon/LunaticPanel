namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageManifestFailedReadingException : InfrastructureCodedException
{
    public PackageManifestFailedReadingException() :
        base(nameof(PackageManifestFailedReadingException), "Failed to read manifest from lpkg.")
    {
    }
}
