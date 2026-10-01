namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageCopyright
{
    public PackageCopyright(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
