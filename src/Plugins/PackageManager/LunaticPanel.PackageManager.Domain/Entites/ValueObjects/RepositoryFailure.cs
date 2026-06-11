namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record RepositoryFailure
{
    public string Value { get; }
    public RepositoryFailure(string value)
    {
        Value = value;
        //TODO: VALIDATE
    }

}
