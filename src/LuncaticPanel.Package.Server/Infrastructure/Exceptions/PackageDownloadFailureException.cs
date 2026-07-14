namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageDownloadFailureException : InfrastructureCodedException
{
    public PackageDownloadFailureException(string statusCode, string? message) :
        base(nameof(PackageDownloadFailureException), $"Package download failed with '{(message ?? "No Message")}' ({statusCode}")
    {
    }
}
