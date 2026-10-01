namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageDownloadPathTooLongException : InfrastructureCodedException
{
    public PackageDownloadPathTooLongException() :
        base(nameof(PackageDownloadPathTooLongException), "The output path is too long to download the package")
    {
    }
}
