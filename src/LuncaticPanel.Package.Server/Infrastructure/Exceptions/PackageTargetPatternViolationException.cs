namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class PackageTargetPatternViolationException : InfrastructureCodedException
{
    public PackageTargetPatternViolationException() :
        base(nameof(PackageTargetPatternViolationException), "Target is not a valid URL")
    {
    }
}
