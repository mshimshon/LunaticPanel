using LunaticPanel.PackageManager.Domain.Entities.Exceptions;
using System.Text.RegularExpressions;

namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackagePanelVersion
{
    public string Value { get; }
    private static readonly Regex _regexValidator = new(@"^\d+\.\d+\.\d+(-[A-Za-z0-9\-]+\.\d+)?$", RegexOptions.Compiled);

    public PackagePanelVersion(int major) : this(major, 0, 0) { }
    public PackagePanelVersion(int major, int minor) : this(major, minor, 0) { }

    public PackagePanelVersion(int major, int minor, int patch) : this($"{major}.{minor}.{patch}")
    {
    }

    public PackagePanelVersion(string version)
    {
        if (!_regexValidator.IsMatch(version))
            throw new PackageVersionInvalidException(version);
        Value = version;
    }

}
