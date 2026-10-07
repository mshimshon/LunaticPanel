namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackageAutoUpdateScore
{
    public int Value { get; }
    public PackageAutoUpdateScore(int value)
    {
        Value = value;
    }
}
