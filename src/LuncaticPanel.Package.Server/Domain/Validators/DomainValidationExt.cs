namespace LuncaticPanel.Package.Server.Domain.Validators;

public static class DomainValidationExt
{
    public const string PKG_ID_VALIDATION_PATTERN = @"^[a-zA-Z]+(\.[a-zA-Z]+)*$";
    public const string ASCI_STRICT_VALIDATION_PATTERN = @"^[\x00-\x7F]+$";
    public const string ASCI_PRINTABLE_VALIDATION_PATTERN = @"^[\x20-\x7E]+$";
    public const string ALPHANUM_VALIDATION_PATTERN = @"^[a-zA-Z0-9]+$";
    public const string ALPHANUM_INCLSPACEDOT_VALIDATION_PATTERN = @"^[a-zA-Z0-9 .,]+$";
    public const string DOTNET_VERSION_VALIDATION_PATTERN = @"^(?:[0-9]|[1-9][0-9]{1,2}|1000)$";
    public const string PANEL_VERSION_VALIDATION_PATTERN = DOTNET_VERSION_VALIDATION_PATTERN;
    public const string PKG_VERSION_VALIDATION_PATTERN = @"^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$";
    public const string PKG_ENTRYFILE_VALIDATION_PATTERN = @"^[^\\/:*?""<>|]+\.dll$";
    public const int PKG_TITLE_MIN_LENGTH = 1;
    public const int PKG_TITLE_MAX_LENGTH = 256;
    public const int PKG_ID_MIN_LENGTH = PKG_TITLE_MIN_LENGTH;
    public const int PKG_ID_MAX_LENGTH = PKG_TITLE_MAX_LENGTH;
    public const int PKG_DESC_MIN_LENGTH = 1;
    public const int PKG_DESC_MAX_LENGTH = 4000;
    public const int PKG_AUTHOR_MIN_LENGTH = 1;
    public const int PKG_AUTHOR_MAX_LENGTH = 256;
    public const int PKG_COPYRIGHT_MIN_LENGTH = 1; // TODO: IMPLEMENT VALIDATION ON DOMAIN
    public const int PKG_COPYRIGHT_MAX_LENGTH = 256;


}

