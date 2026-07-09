using LuncaticPanel.Package.Server.Application.Validators;
using LuncaticPanel.Package.Server.Application.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageTitle
{
    public PackageTitle(string value)
    {
        if (value.Length < DomainValidationExt.PKG_TITLE_MIN_LENGTH || value.Length > DomainValidationExt.PKG_TITLE_MAX_LENGTH)
            throw new PackageTitleLengthException();
        else if (string.IsNullOrWhiteSpace(value))
            throw new PackageTitleNullException();
        else if (!Regex.IsMatch(value, DomainValidationExt.ALPHANUM_VALIDATION_PATTERN))
            throw new PackageTitlePatternViolationException(value);

        Value = value;
    }

    public string Value { get; }
}
