namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageDescription
{
    public string Value { get; }
    public PackageDescription(string value)
    {
        Value = value;
    }
}
