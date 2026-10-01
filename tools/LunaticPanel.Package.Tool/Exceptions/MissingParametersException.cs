using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions;

internal class MissingParametersException : HostCodedException
{
    public MissingParametersException(string message) :
        base(nameof(MissingParametersException), message)
    {
    }
}
