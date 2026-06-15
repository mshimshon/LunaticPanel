using LunaticPanel.PackageManager.Domain.Entites.Enums;

namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record RepositorySourceInfo
{
    public RepositorySource Source { get; init; }
    public RepositorySourceType SourceType { get; init; }
    public RepositorySourceInfo(RepositorySource source, RepositorySourceType sourceType)
    {
        Source = source;
        SourceType = sourceType;
    }

}
