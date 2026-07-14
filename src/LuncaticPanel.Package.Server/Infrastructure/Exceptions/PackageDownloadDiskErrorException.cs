namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageDownloadDiskErrorException : InfrastructureCodedException
{
    public PackageDownloadDiskErrorException() :
        base(nameof(PackageDownloadDiskErrorException), "Cannot Download package due to disk error, file lock or other sharing issues.")
    {
    }
}
