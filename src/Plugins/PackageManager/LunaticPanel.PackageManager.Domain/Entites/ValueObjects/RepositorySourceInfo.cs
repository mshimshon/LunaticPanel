using LunaticPanel.PackageManager.Domain.Entites.Enums;

namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record RepositorySourceInfo
{
    public RepositorySource Source { get; }
    public RepositorySourceType SourceType { get; }
    public RepositorySourceInfo(RepositorySource source, RepositorySourceType sourceType)
    {
        Source = source;
        SourceType = sourceType;
    }

}
