using LuncaticPanel.Package.Server.Application.Validators;
using LuncaticPanel.Package.Server.Application.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public record PackageEndOfLifeMessage
{
    public PackageEndOfLifeMessage(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackageEndOfLifeMessageEmptyException();
        else if (!Regex.IsMatch(value, DomainValidationExt.ASCI_STRICT_VALIDATION_PATTERN))
            throw new PackageEndOfLifeMessageViolationException();
        Value = value;
    }

    public string Value { get; }
}
