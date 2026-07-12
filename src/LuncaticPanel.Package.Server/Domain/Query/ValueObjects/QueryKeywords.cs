using LuncaticPanel.Package.Server.Domain.Validators;
using LuncaticPanel.Package.Server.Domain.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Domain.Query.ValueObjects;

public sealed record QueryKeywords
{
    public string[] Value { get; }
    public QueryKeywords(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new QueryKeywordsEmptyException();
        else if (Regex.IsMatch(value, DomainValidationExt.ALPHANUM_INCLSPACEDOT_VALIDATION_PATTERN, RegexOptions.IgnoreCase))
            throw new QueryKeywordsViolationException();

        Value = value.Split(' ');

    }

}
