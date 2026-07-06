using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions.ValidateExceptions;

internal class ValidateInputInvalidException : HostCodedException
{
    public ValidateInputInvalidException(string input) :
        base(nameof(ValidateInputInvalidException), $"'{input}' is invalid directory/file or does not exist.")
    {
    }
}
