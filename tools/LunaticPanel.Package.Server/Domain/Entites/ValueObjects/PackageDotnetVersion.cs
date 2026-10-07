using LunaticPanel.Package.Server.Domain.Validators;
using LunaticPanel.Package.Server.Domain.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LunaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageDotnetVersion
{
    public PackageDotnetVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackageDotnetVersionRequiredException();
        else if (!Regex.IsMatch(value, DomainValidationExt.DOTNET_VERSION_VALIDATION_PATTERN))
            throw new PackageDotnetVersionInvalidException(value);
        Value = value;
    }

    public string Value { get; }
}
