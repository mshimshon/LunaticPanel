using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entites;

public sealed record RepositorySourceEntity
{
    public RepositorySourceName Name { get; }
    public RepositorySourceInfo Info { get; }
    public RepositorySourceEntity(RepositorySourceName name, RepositorySourceInfo info)
    {
        Name = name;
        Info = info;
    }
}
