using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

public class DotnetSDKNotFoundException : HostCodedException
{
    public DotnetSDKNotFoundException(string location) :
        base(nameof(DotnetSDKNotFoundException), $"The dotnet SDK was not detected at {location}")
    {
    }
}
