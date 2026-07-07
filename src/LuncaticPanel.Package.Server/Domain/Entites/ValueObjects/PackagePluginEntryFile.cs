namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackagePluginEntryFile
{
    public PackagePluginEntryFile(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
