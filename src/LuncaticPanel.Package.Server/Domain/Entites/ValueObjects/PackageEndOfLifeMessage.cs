namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public record PackageEndOfLifeMessage
{
    public PackageEndOfLifeMessage(string value)
    {
        Value = value;
    }

    public string Value { get; }
}
