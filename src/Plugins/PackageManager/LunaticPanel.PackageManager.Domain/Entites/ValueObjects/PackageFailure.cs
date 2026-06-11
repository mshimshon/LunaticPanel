namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageFailure
{
    public PackageFailure(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
