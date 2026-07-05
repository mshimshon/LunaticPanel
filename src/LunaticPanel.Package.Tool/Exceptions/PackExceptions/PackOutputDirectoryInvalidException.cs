using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions.PackExceptions;

internal class PackOutputDirectoryInvalidException : HostCodedException
{
    public PackOutputDirectoryInvalidException(string path) :
        base(nameof(PackOutputDirectoryInvalidException), $"{path} is not a directory, the output must be a directory.")
    {
    }
}
