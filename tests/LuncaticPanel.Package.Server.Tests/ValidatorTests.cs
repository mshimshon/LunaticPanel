using LuncaticPanel.Package.Server.Domain.Validators;
using System.Text.RegularExpressions;

namespace LuncaticPanel.Package.Server.Tests;

public class ValidatorTests
{
    [Theory]
    [InlineData("plugin.dll")]
    [InlineData("MyPlugin.dll")]
    [InlineData("core.module.dll")]
    [InlineData("ABC123.dll")]
    public void PkgEntryFile_ShouldValidate(string file)
    {
        bool valid = Regex.IsMatch(file, DomainValidationExt.PKG_ENTRYFILE_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("plugin")]            // missing .dll
    [InlineData("plugin.exe")]        // wrong extension
    [InlineData("plug/in.dll")]       // slash
    [InlineData("plug\\in.dll")]      // backslash
    [InlineData("plug:in.dll")]       // colon
    [InlineData("plug*in.dll")]       // asterisk
    [InlineData("plug?in.dll")]       // question mark
    [InlineData("plug\"in.dll")]      // quote
    [InlineData("plug<in.dll")]       // <
    [InlineData("plug>in.dll")]       // >
    [InlineData("plug|in.dll")]       // |
    [InlineData(".dll")]              // empty name before extension
    public void PkgEntryFile_ShouldNOTValidate(string file)
    {
        bool valid = Regex.IsMatch(file, DomainValidationExt.PKG_ENTRYFILE_VALIDATION_PATTERN);
        Assert.False(valid);
    }

    [Theory]
    [InlineData("Alpha")]
    [InlineData("Alpha.Beta")]
    [InlineData("Core.System.Module")]
    [InlineData("A.B.C")]
    public void PkgId_ShouldValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.PKG_ID_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("alpha1")]
    [InlineData("alpha.beta1")]
    [InlineData("alpha_beta")]
    [InlineData("alpha-beta")]
    [InlineData("alpha..beta")]
    [InlineData(".alpha")]
    [InlineData("alpha.")]
    [InlineData("alpha beta")]
    public void PkgId_ShouldNOTValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.PKG_ID_VALIDATION_PATTERN);
        Assert.False(valid);
    }

    [Theory]
    [InlineData("Hello")]
    [InlineData("123")]
    [InlineData("!@#$")]
    [InlineData("\t")]      // tab is allowed
    [InlineData("\n")]      // newline is allowed
    [InlineData("\r")]      // carriage return is allowed
    [InlineData("\x00")]    // null byte is allowed
    public void AsciiStrict_ShouldValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ASCI_STRICT_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("ă")]       // non‑ASCII
    [InlineData("€")]       // non‑ASCII
    [InlineData("🙂")]      // emoji
    [InlineData("𐍈")]      // outside BMP
    public void AsciiStrict_ShouldNOTValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ASCI_STRICT_VALIDATION_PATTERN);
        Assert.False(valid);
    }
    [Theory]
    [InlineData("Hello")]
    [InlineData("Hello World")]
    [InlineData("123456")]
    [InlineData("!@#$%^&*()")]
    [InlineData("[]{}<>?/\\|")]
    [InlineData("~`")]
    public void AsciiPrintable_ShouldValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ASCI_PRINTABLE_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("\t")]          // tab
    [InlineData("\n")]          // newline
    [InlineData("\r")]          // carriage return
    [InlineData("ă")]           // non‑ASCII
    [InlineData("€")]           // non‑ASCII
    [InlineData("🙂")]          // emoji
    public void AsciiPrintable_ShouldNOTValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ASCI_PRINTABLE_VALIDATION_PATTERN);
        Assert.False(valid);
    }

    [Theory]
    [InlineData("ABC")]
    [InlineData("abc")]
    [InlineData("A1B2C3")]
    [InlineData("string")]
    [InlineData("123")]
    public void Alphanum_ShouldValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ALPHANUM_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("abc def")]
    [InlineData("a.b")]
    [InlineData("a-b")]
    [InlineData("a_b")]
    [InlineData("a!")]
    public void Alphanum_ShouldNOTValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ALPHANUM_VALIDATION_PATTERN);
        Assert.False(valid);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("abc def")]
    [InlineData("A B C")]
    [InlineData("file.name")]
    [InlineData("Version 1.0")]
    [InlineData("Hello World 2.0")]
    public void AlphanumSpaceDot_ShouldValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ALPHANUM_INCLSPACEDOT_VALIDATION_PATTERN, RegexOptions.IgnoreCase);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("abc-def")]   // hyphen
    [InlineData("a_b")]       // underscore
    [InlineData("a!")]        // punctuation
    [InlineData("a?")]
    [InlineData("a,")]
    [InlineData("a:")]
    [InlineData("a/")]
    [InlineData("a\\")]
    [InlineData("a@")]
    [InlineData("a#")]
    public void AlphanumSpaceDot_ShouldNOTValidate(string value)
    {
        bool valid = Regex.IsMatch(value, DomainValidationExt.ALPHANUM_INCLSPACEDOT_VALIDATION_PATTERN, RegexOptions.IgnoreCase);
        Assert.False(valid);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("6")]
    [InlineData("8")]
    [InlineData("1000")]
    public void PackagePanelVersion_ShouldValidate(string version)
    {
        bool valid = Regex.IsMatch(version, DomainValidationExt.PANEL_VERSION_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("1001")]
    [InlineData("10000")]
    [InlineData("10.0.0")]
    [InlineData("3.1")]
    public void PackagePanelVersion_ShouldNOTValidate(string version)
    {
        bool valid = Regex.IsMatch(version, DomainValidationExt.PANEL_VERSION_VALIDATION_PATTERN);
        Assert.False(valid);
    }

    [Theory]
    [InlineData("10")]
    [InlineData("6")]
    [InlineData("8")]
    [InlineData("1000")]
    public void PackageDotnetVersion_ShouldValidate(string version)
    {
        bool valid = Regex.IsMatch(version, DomainValidationExt.DOTNET_VERSION_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("1001")]
    [InlineData("10000")]
    [InlineData("10.0.0")]
    [InlineData("3.1")]
    public void PackageDotnetVersion_ShouldNOTValidate(string version)
    {
        bool valid = Regex.IsMatch(version, DomainValidationExt.DOTNET_VERSION_VALIDATION_PATTERN);
        Assert.False(valid);
    }

    [Theory]
    [InlineData("1.0.0")]
    [InlineData("0.1.5")]
    [InlineData("10.20.30")]
    [InlineData("999.999.999")]
    public void PkgVersion_ShouldValidate(string version)
    {
        bool valid = Regex.IsMatch(version, DomainValidationExt.PKG_VERSION_VALIDATION_PATTERN);
        Assert.True(valid);
    }

    [Theory]
    [InlineData("01.0.0")]       // leading zero in major
    [InlineData("1.01.0")]       // leading zero in minor
    [InlineData("1.0.01")]       // leading zero in patch
    [InlineData("1.0")]          // missing segment
    [InlineData("1")]            // missing segments
    [InlineData("1.0.0.1")]      // too many segments
    [InlineData("1.0.0-beta")]   // prerelease not allowed
    [InlineData("1.0.0+build")]  // metadata not allowed
    [InlineData("a.b.c")]        // non-numeric
    [InlineData("1.0.x")]        // non-numeric
    public void PkgVersion_ShouldNOTValidate(string version)
    {
        bool valid = Regex.IsMatch(version, DomainValidationExt.PKG_VERSION_VALIDATION_PATTERN);
        Assert.False(valid);
    }
}
