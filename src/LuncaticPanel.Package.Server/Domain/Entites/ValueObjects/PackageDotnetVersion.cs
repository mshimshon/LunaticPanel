namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageDotnetVersion
{
    public PackageDotnetVersion(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
