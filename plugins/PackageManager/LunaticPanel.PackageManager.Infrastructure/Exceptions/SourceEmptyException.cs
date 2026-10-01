using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;

public class SourceEmptyException : HostCodedException
{
    public SourceEmptyException() :
        base(nameof(SourceEmptyException), "No sources found.")
    {
    }
}
