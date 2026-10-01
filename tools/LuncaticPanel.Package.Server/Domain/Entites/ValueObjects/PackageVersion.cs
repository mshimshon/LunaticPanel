using LuncaticPanel.Package.Server.Domain.Validators;
using LuncaticPanel.Package.Server.Domain.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageVersion
{
    public PackageVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackageVersionRequiredException();
        else if (!Regex.IsMatch(value, DomainValidationExt.PKG_VERSION_VALIDATION_PATTERN))
            throw new PackageVersionFormatViolationException(value);
        Value = value;
    }

    public string Value { get; }
}
