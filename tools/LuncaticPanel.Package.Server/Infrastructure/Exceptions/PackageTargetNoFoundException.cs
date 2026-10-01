namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageTargetNoFoundException : InfrastructureCodedException
{
    public PackageTargetNoFoundException() :
        base(nameof(PackageTargetNoFoundException), "Could not locate package at the given target.")
    {
    }
}
