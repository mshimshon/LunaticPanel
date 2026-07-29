namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record RepositorySourceRemote : RepositorySource
{
    public override string Value { get; }

    public RepositorySourceRemote(string value)
    {
        Value = value;
    }
}
