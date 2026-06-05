namespace LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

public sealed record PackageVersion : IComparable<PackageVersion>
{
    private readonly Version _version;
    public string Value { get; }

    public PackageVersion(int major) : this(major, 0, 0) { }
    public PackageVersion(int major, int minor) : this(major, minor, 0) { }

    public PackageVersion(int major, int minor, int patch)
    {
        _version = new Version(major, minor, patch);
        Value = _version.ToString();
    }

    public int CompareTo(PackageVersion? other)
    {
        if (other is null) return 1;
        return _version.CompareTo(other._version);
    }

    public static bool operator >(PackageVersion left, PackageVersion right)
        => left.CompareTo(right) > 0;

    public static bool operator <(PackageVersion left, PackageVersion right)
        => left.CompareTo(right) < 0;

    public static bool operator >=(PackageVersion left, PackageVersion right)
        => left.CompareTo(right) >= 0;

    public static bool operator <=(PackageVersion left, PackageVersion right)
        => left.CompareTo(right) <= 0;

}
