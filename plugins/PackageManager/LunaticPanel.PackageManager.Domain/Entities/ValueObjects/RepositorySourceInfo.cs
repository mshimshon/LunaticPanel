using LunaticPanel.PackageManager.Domain.Entities.Enums;

namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

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
