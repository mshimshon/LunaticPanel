using LunaticPanel.Package.Server.Domain.Validators;
using LunaticPanel.Package.Server.Domain.Validators.Exceptions;
using System.Text.RegularExpressions;

namespace LunaticPanel.Package.Server.Domain.Entites.ValueObjects;

public sealed record PackagePluginEntryFile
{
    public PackagePluginEntryFile(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new PackageEntryFileRequiredException();
        else if (!Regex.IsMatch(value, DomainValidationExt.PKG_ENTRYFILE_VALIDATION_PATTERN, RegexOptions.IgnoreCase))
            throw new PackageEntryFileDllViolationException(value);
        Value = value;
    }

    public string Value { get; }
}
