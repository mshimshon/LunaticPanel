namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageValidationFailedNoDataException : InfrastructureCodedException
{
    public PackageValidationFailedNoDataException() :
        base(nameof(PackageValidationFailedNoDataException), "The validator tool did not return any manifest.")
    {
    }
}
