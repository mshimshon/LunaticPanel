using LunaticPanel.PackageManager.Domain.Entites.Enums;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entites;

public sealed record RepositorySourceEntity
{
    public RepositorySourceName Name { get; }
    public RepositorySourceInfo Info { get; }
    public RepositoryFailure? Failure { get; }
    public RepositorySourceState State { get; }
    public RepositorySourceEntity(RepositorySourceName name, RepositorySourceInfo info, RepositorySourceState state)
    {
        Name = name;
        Info = info;
        State = state;
    }

    public RepositorySourceEntity(RepositorySourceName name, RepositorySourceInfo info, RepositoryFailure failure, RepositorySourceState state)
        : this(name, info, state)
    {
        Failure = failure;
    }

}
