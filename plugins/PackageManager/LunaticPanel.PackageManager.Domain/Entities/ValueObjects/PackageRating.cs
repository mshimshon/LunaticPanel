namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackageRating
{
    public int Value { get; }
    public PackageRating(int value)
    {
        Value = value;
    }
}
