namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageTargetEmptyException : InfrastructureCodedException
{
    public PackageTargetEmptyException() :
        base(nameof(PackageTargetEmptyException), "Target location cannot by empty or null.")
    {
    }
}
