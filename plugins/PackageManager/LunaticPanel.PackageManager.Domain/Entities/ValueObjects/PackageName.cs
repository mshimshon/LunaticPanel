namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackageName
{
    public string Value { get; }
    public PackageName(string value)
    {
        Value = value;
    }
}
