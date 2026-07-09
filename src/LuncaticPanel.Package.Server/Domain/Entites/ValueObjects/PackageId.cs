using LuncaticPanel.Package.Server.Application.Validators;
using LuncaticPanel.Package.Server.Application.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageId
{
    public PackageId(string value)
    {
        if (value.Length < DomainValidationExt.PKG_ID_MIN_LENGTH || value.Length > DomainValidationExt.PKG_ID_MAX_LENGTH)
            throw new PackageIdLengthException();
        else if (string.IsNullOrWhiteSpace(value))
            throw new PackageIdNullException();
        else if (!Regex.IsMatch(value, DomainValidationExt.PKG_ID_VALIDATION_PATTERN))
            throw new PackageIdPatternViolationException(value);
        Value = value;
    }

    public string Value { get; }
}
