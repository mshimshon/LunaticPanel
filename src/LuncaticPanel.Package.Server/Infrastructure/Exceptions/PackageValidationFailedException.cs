namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public class PackageValidationFailedException : InfrastructureCodedException
{
    public PackageValidationFailedException(string code, string message) :
        base(nameof(PackageValidationFailedException), $"{code}:[{message}]")
    {
    }
}
