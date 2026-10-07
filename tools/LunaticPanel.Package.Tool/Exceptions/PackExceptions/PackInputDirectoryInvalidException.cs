using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions.PackExceptions;

internal class PackInputDirectoryInvalidException : HostCodedException
{
    public PackInputDirectoryInvalidException(string path) :
        base(nameof(PackInputDirectoryInvalidException), $"{path} does not exist.")
    {
    }
}
