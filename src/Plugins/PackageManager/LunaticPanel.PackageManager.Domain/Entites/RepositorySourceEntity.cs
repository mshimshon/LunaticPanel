using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entites;

public sealed record RepositorySourceEntity
{
    public RepositorySourceName Name { get; init; }
    public RepositorySource Source { get; }
    public RepositorySourceType SourceType { get; }
    public RepositorySourceEntity(RepositorySourceName name, RepositorySourceLocal source)
    {
        Source = source;
        SourceType = RepositorySourceType.Local;
        Name = name;
    }

    public RepositorySourceEntity(RepositorySourceName name, RepositorySourceRemote source)
    {
        Source = source;
        SourceType = RepositorySourceType.Remote;
        Name = name;
    }


}
