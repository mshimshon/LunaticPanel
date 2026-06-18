using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

public class BootstrapNotFoundException : HostCodedException
{
    public BootstrapNotFoundException() : base(nameof(BootstrapNotFoundException), "Couldn't locate bootstrap file, this is a critical error.")
    { }
}
