namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageDescription
{
    public PackageDescription(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
