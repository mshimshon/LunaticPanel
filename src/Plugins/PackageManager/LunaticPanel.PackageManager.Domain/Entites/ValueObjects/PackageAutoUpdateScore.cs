namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageAutoUpdateScore
{
    public int Value { get; }
    public PackageAutoUpdateScore(int value)
    {
        Value = value;
    }
}
