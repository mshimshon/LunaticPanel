namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackageDescription
{
    public string Value { get; }
    public PackageDescription(string value)
    {
        Value = value;
    }
}
