using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

public class PushInputFileMisingException : HostCodedException
{
    public PushInputFileMisingException() :
        base(nameof(PushInputFileMisingException), "Input missing or LPKG not found.")
    {
    }
}
