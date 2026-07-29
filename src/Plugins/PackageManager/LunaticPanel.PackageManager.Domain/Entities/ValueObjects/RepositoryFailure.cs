namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record RepositoryFailure
{
    public string Value { get; }
    public RepositoryFailure(string value)
    {
        Value = value;
        //TODO: VALIDATE
    }

}
