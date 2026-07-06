using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions.UnpackExceptions;

internal class UnpackOutputDirectoryInvalidException : HostCodedException
{
    public UnpackOutputDirectoryInvalidException(string path) :
        base(nameof(UnpackOutputDirectoryInvalidException), $"{path} is not a directory, the output must be a directory.")
    {
    }
}
