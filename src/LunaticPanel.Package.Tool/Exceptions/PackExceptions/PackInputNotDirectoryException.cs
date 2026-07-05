using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions.PackExceptions;

internal class PackInputNotDirectoryException : HostCodedException
{
    public PackInputNotDirectoryException(string? input) : base(nameof(PackInputNotDirectoryException), $"--input is not a directory '{input ?? "null"}'")
    {
    }
}
