using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Application.Validators;

public static class DomainValidationExt
{
    public const string PKG_ID_VALIDATION_PATTERN = @"^[a-zA-Z]+(\.[a-zA-Z]+)*$";
    public const string ASCI_STRICT_VALIDATION_PATTERN = @"^[\x00-\x7F]+$";
    public const string ASCI_PRINTABLE_VALIDATION_PATTERN = @"^[\x20-\x7E]+$";
    public const string ALPHANUM_VALIDATION_PATTERN = @"^[a-zA-Z0-9]+$";
    public const string DOTNET_VERSION_VALIDATION_PATTERN = @"^(?:[0-9]|[1-9][0-9]{1,2}|1000)$";
    public const string PANEL_VERSION_VALIDATION_PATTERN = DOTNET_VERSION_VALIDATION_PATTERN;
    public const string PKG_VERSION_VALIDATION_PATTERN = @"/^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$/";
    public const string PKG_ENTRYFILE_VALIDATION_PATTERN = @"^[^\\/:*?""<>|]+\.dll$";
    public const int PKG_TITLE_MIN_LENGTH = 1;
    public const int PKG_TITLE_MAX_LENGTH = 256;
    public const int PKG_ID_MIN_LENGTH = PKG_TITLE_MIN_LENGTH;
    public const int PKG_ID_MAX_LENGTH = PKG_TITLE_MAX_LENGTH;
    public const int PKG_DESC_MIN_LENGTH = 1;
    public const int PKG_DESC_MAX_LENGTH = 4000;
    public const int PKG_AUTHOR_MIN_LENGTH = 1;
    public const int PKG_AUTHOR_MAX_LENGTH = 256;

    public static bool ValidateDescription(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        if (!Regex.IsMatch(value, ASCI_STRICT_VALIDATION_PATTERN))
            return false;
        return true;
    }


    public static bool ValidateDotnetVersion(string data)
    {
        if (string.IsNullOrWhiteSpace(data)) return false;
        if (!Regex.IsMatch(data, DOTNET_VERSION_VALIDATION_PATTERN))
            return false;
        return true;
    }

    public static bool ValidatePanelVersion(string data)
        => ValidateDotnetVersion(data);
}

