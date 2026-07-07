namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageId
{
    public PackageId(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
