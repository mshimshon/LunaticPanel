using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Package.Tool.Exceptions.InfoExceptions;

public class InfoInputFileNotExistException : HostCodedException
{
    public InfoInputFileNotExistException() :
        base(nameof(InfoInputFileNotExistException), "File does not exist.")
    {
    }
}
