namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record RepositorySourceLocal : RepositorySource
{
    public override string Value { get; }

    public RepositorySourceLocal(string value)
    {
        Value = value;
    }
}
