using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions.UnpackExceptions;

internal class UnpackInputFileMissingException : HostCodedException
{
    public UnpackInputFileMissingException(string input) :
        base(nameof(UnpackInputFileMissingException), $"{input} does not exist.")
    {
    }
}
