namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record RepositorySourceName
{
    public string Value { get; }

    public RepositorySourceName(string value)
    {
        Value = value;
    }
}