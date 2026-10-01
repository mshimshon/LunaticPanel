namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageDownloadOuputNullOrInvalidException : InfrastructureCodedException
{
    public PackageDownloadOuputNullOrInvalidException() :
        base(nameof(PackageDownloadOuputNullOrInvalidException), "The output path to download package is either null or contain invalid characters.")
    {
    }
}
