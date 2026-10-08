using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

public class PushAccessResponseInvalidException : HostCodedException
{
    public PushAccessResponseInvalidException(string message) : base(nameof(PushAccessResponseInvalidException), message)
    {
    }
}
