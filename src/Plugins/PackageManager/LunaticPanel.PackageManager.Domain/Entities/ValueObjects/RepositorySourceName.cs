namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record RepositorySourceName
{
    public string Value { get; }

    public RepositorySourceName(string value)
    {
        Value = value;
    }
}