using LunaticPanel.PackageManager.Domain.Entities.Exceptions;

namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackagePanelVersion : IComparable<PackagePanelVersion>
{
    private readonly Version _version;
    public string Value { get; }

    public PackagePanelVersion(int major) : this(major, 0, 0) { }
    public PackagePanelVersion(int major, int minor) : this(major, minor, 0) { }

    public PackagePanelVersion(int major, int minor, int patch)
    {
        _version = new Version(major, minor, patch);
        Value = _version.ToString();
    }

    public PackagePanelVersion(string version)
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

    public int CompareTo(PackagePanelVersion? other)
    {
        if (other is null) return 1;
        return _version.CompareTo(other._version);
    }

    public static bool operator >(PackagePanelVersion left, PackagePanelVersion right)
        => left.CompareTo(right) > 0;

    public static bool operator <(PackagePanelVersion left, PackagePanelVersion right)
        => left.CompareTo(right) < 0;

    public static bool operator >=(PackagePanelVersion left, PackagePanelVersion right)
        => left.CompareTo(right) >= 0;

    public static bool operator <=(PackagePanelVersion left, PackagePanelVersion right)
        => left.CompareTo(right) <= 0;

}
