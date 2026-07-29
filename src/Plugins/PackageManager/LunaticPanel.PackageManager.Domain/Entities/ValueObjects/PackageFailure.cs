namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackageFailure
{
    public PackageFailure(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
