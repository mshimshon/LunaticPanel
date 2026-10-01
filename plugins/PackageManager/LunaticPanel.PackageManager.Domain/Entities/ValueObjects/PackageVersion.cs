using LunaticPanel.PackageManager.Domain.Entities.Exceptions;

namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

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

    public PackageVersion(string version)
    {

        var split = version.Split('+')[0].Split('.');
        if (split.Length == 1)
            _version = new Version(int.Parse(split[0]), 0, 0);
        else if (split.Length == 2)
            _version = new Version(int.Parse(split[0]), int.Parse(split[1]), 0);
        else if (split.Length == 3)
            _version = new Version(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
        else
            throw new PackageVersionInvalidException(version);
        Value = _version!.ToString();
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
