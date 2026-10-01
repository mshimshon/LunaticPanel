using LunaticPanel.PackageManager.Domain.Exceptions;

namespace LunaticPanel.PackageManager.Domain.Entities.Exceptions;

public class RepositorySourceRemoteNotUrlException : DomainException
{
    public RepositorySourceRemoteNotUrlException() :
        base(nameof(RepositorySourceRemoteNotUrlException), "Repository Source Remote is not a valid URL.")
    {
    }
}
