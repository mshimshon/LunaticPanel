namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record RepositorySourceLocal : RepositorySource
{
    public override string Value { get; }

    public RepositorySourceLocal(string value)
    {
        Value = value;
    }
}
