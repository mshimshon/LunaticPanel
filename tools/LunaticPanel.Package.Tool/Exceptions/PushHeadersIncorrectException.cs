using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

public class PushHeadersIncorrectException : HostCodedException
{
    public PushHeadersIncorrectException() :
        base(nameof(PushHeadersIncorrectException), "--headers \"key:value;key:value\" is correct format.")
    {
    }
}
