using LunaticPanel.Package.Server.Domain.Validators.Exceptions;

namespace LunaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageDotnetVersion
{
    public PackageDotnetVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackageDotnetVersionRequiredException();
        Value = value;
    }

    public string Value { get; }
}
