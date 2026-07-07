namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageVersion
{
    public PackageVersion(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
