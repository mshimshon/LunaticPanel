namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public class PackageValidationFailedUnknownException : InfrastructureCodedException
{
    public PackageValidationFailedUnknownException() :
        base(nameof(PackageValidationFailedUnknownException), "Failed to validate lpkg due to unknown error.")
    {
    }
}
