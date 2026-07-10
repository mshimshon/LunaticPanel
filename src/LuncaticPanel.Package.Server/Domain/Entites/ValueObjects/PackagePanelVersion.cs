using LuncaticPanel.Package.Server.Application.Validators;
using LuncaticPanel.Package.Server.Application.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackagePanelVersion
{
    public PackagePanelVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackagePanelVersionRequiredException();
        else if (Regex.IsMatch(value, DomainValidationExt.PANEL_VERSION_VALIDATION_PATTERN))
            throw new PackagePanelVersionInvalidException(value);
        Value = value;
    }

    public string Value { get; }
}
