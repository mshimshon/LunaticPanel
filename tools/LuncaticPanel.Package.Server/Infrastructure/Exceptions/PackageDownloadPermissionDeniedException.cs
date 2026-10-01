namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageDownloadPermissionDeniedException : InfrastructureCodedException
{
    public PackageDownloadPermissionDeniedException() :
        base(nameof(PackageDownloadPermissionDeniedException), "Write Permission denied cannot download package file.")
    {
    }
}
