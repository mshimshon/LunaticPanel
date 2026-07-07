namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackagePanelVersion
{
    public PackagePanelVersion(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
