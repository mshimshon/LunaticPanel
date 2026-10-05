using LunaticPanel.Package.Server.Domain.Validators;
using LunaticPanel.Package.Server.Domain.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LunaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackagePanelVersion
{
    public PackagePanelVersion(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackagePanelVersionRequiredException();
        else if (!Regex.IsMatch(value, DomainValidationExt.PANEL_VERSION_VALIDATION_PATTERN))
            throw new PackagePanelVersionInvalidException(value);
        Value = value;
    }

    public string Value { get; }
}
