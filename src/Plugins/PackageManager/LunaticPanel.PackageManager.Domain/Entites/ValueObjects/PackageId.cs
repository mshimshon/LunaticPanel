namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageId
{
    public string Value { get; }
    public PackageId(string value)
    {
        Value = value;
        // TODO: Validate PackageId Format
    }

}

