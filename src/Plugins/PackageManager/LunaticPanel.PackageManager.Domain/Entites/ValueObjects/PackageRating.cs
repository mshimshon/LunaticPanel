namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageRating
{
    public int Value { get; }
    public PackageRating(int value)
    {
        Value = value;
    }
}
