namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageTitle
{
    public PackageTitle(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
