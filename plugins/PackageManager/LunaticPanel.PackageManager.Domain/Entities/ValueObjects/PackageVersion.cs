using LunaticPanel.PackageManager.Domain.Entities.Exceptions;
using System.Text.RegularExpressions;

namespace LunaticPanel.PackageManager.Domain.Entities.ValueObjects;

public sealed record PackageVersion
{
    public string Value { get; }
    private static readonly Regex _regexValidator = new(@"^\d+\.\d+\.\d+(-[0-9A-Za-z\-\.]+)?$", RegexOptions.Compiled);

    public PackageVersion(int major) : this(major, 0, 0) { }
    public PackageVersion(int major, int minor) : this(major, minor, 0) { }

    public PackageVersion(int major, int minor, int patch) : this($"{major}.{minor}.{patch}")
    {
    }

    public PackageVersion(string version)
    {
        if (!_regexValidator.IsMatch(version))
            throw new PackageVersionInvalidException(version);
        Value = version;
    }
}
