using LuncaticPanel.Package.Server.Domain.Validators;
using LuncaticPanel.Package.Server.Domain.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackageAuthor
{
    public PackageAuthor(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackageAuthorRequiredException();
        else if (value.Length < DomainValidationExt.PKG_AUTHOR_MIN_LENGTH || value.Length > DomainValidationExt.PKG_AUTHOR_MAX_LENGTH)
            throw new PackageAuthorLengthException();
        else if (!Regex.IsMatch(value, DomainValidationExt.ALPHANUM_VALIDATION_PATTERN))
            throw new PackageAuthorViolationException(value);
        Value = value;
    }

    public string Value { get; }
}
