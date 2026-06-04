namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record RepositorySourceRemote : RepositorySource
{
    public override string Value { get; }

    public RepositorySourceRemote(string value)
    {
        Value = value;
    }
}
