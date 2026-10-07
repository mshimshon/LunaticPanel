using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

namespace LunaticPanel.PackageManager.Domain.Entities;

public sealed record RepositorySourceEntity
{
    public RepositorySourceName Name { get; init; }
    public RepositorySourceInfo Info { get; init; }
    public RepositoryFailure? Failure { get; init; }
    public RepositorySourceState State { get; init; }
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
