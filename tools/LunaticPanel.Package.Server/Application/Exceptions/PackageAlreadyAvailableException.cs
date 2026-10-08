namespace LunaticPanel.Package.Server.Application.Exceptions;

public class PackageAlreadyAvailableException : AppLayerException
{
    public PackageAlreadyAvailableException() : base(nameof(PackageAlreadyAvailableException), "Package of this version already exist.")
    {
    }
}
