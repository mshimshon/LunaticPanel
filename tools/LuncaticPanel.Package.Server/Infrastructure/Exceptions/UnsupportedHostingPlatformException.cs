namespace LuncaticPanel.Package.Server.Infrastructure.Exceptions;

public sealed class UnsupportedHostingPlatformException : InfrastructureCodedException
{
    public UnsupportedHostingPlatformException() :
        base(nameof(UnsupportedHostingPlatformException), "Linux is the only supported platform for running validation tooling.")
    {
    }
}
