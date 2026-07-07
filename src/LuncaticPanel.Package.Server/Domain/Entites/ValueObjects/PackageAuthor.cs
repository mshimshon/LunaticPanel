namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageAuthor
{
    public PackageAuthor(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
