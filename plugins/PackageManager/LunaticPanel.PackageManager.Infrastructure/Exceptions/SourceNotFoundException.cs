using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.PackageManager.Infrastructure.Exceptions;


public class SourceNotFoundException : HostCodedException
{
    public SourceNotFoundException(string source) :
        base(nameof(SourceNotFoundException), $"{source} was not found.")
    {
    }
}
