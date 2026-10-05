using LunaticPanel.Package.Server.Domain.Validators;
using LunaticPanel.Package.Server.Domain.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LunaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageDescription
{
    public PackageDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackageDescriptionNullException();
        else if (!Regex.IsMatch(value, DomainValidationExt.ASCI_STRICT_VALIDATION_PATTERN))
            throw new PackageDescriptionPatternViolationException();
        Value = value;
    }

    public string Value { get; }
}
