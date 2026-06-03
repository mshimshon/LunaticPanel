using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;
using LunaticPanel.Core.Abstraction.Messaging.Extensions;

namespace LunaticPanel.Core.Tests.MessageBus;

public class MessageKeyValidationTests
{

    [Theory]
    [InlineData("enginekey.LunaticPanel.Core.Tests.MessageBus.v12.enabled")]
    [InlineData("enginekey.LunaticPanel.Core.Tests.MessageBus.v992.release")]
    [InlineData("querykey.LunaticPanel.Core.Tests.MessageBus.v2.BETA")]
    [InlineData("querykey.LunaticPanel.Core.Tests.MessageBus.v10.snapshot.sda.sdasd.sdasd.asda.sdasd.as.das.d.dfsdf")]
    [InlineData("querykey.LunaticPanel.Core.Tests.MessageBus.v1000.build")]
    [InlineData("eventkey.LunaticPanel.Core.Tests.MessageBus.v33.FINAL")]
    [InlineData("eventkey.LunaticPanel.Core.Tests.MessageBus.v7.patch")]
    [InlineData("eventkey.LunaticPanel.Core.Tests.MessageBus.v1234.a.a.a")]
    public void MessageKey_PluginIdAssemblyValidation_Should_Succeed(string key)
    {
        Assert.True(MessageKeyValidator.ValidateAssembly(key, "LunaticPanel.core.Tests.MessageBus"));
    }

    [Theory]
    [InlineData("enginekey.core.v12.enabled")]
    [InlineData("querykey.PREFIX.MY.PLUGIN.ID.v2.BETA")]
    [InlineData("querykey.runtime.loader.ext.v10.snapshot.sda.sdasd.sdasd.asda.sdasd.as.das.d.dfsdf")]
    [InlineData("querykey.alpha.beta.gamma.delta.v1000.build")]
    [InlineData("eventkey.PREFIX.MY.PLUGIN.ID.v33.FINAL")]
    [InlineData("eventkey.service.dispatch.router.v7.patch")]
    [InlineData("eventkey.long.prefix.with.many.parts.v1234.a.a.a")]
    public void MessageKey_AllowedPrefixValidation_Should_Succeed(string key)
    {
        Assert.True(MessageKeyValidator.ValidatePrefix(key));
    }

    [Theory]
    [InlineData("enginekey.core.v12.enabled")]
    [InlineData("querykey.PREFIX.MY.PLUGIN.ID.v2.BETA")]
    [InlineData("querykey.runtime.loader.ext.v10.snapshot.sda.sdasd.sdasd.asda.sdasd.as.das.d.dfsdf")]
    [InlineData("querykey.alpha.beta.gamma.delta.v1000.build")]
    [InlineData("eventkey.PREFIX.MY.PLUGIN.ID.v33.FINAL")]
    [InlineData("eventkey.service.dispatch.router.v7.patch")]
    [InlineData("eventkey.long.prefix.with.many.parts.v1234.a.a.a")]
    public void MessageKey_AllowedCharactersValidation_Should_Succeed(string key)
    {
        Assert.True(MessageKeyValidator.ValidateAllowedCharacters(key));
    }

    [Theory]
    [InlineData("enginekey.core.v12.enabled")]
    [InlineData("querykey.PREFIX.MY.PLUGIN.ID.v2.BETA")]
    [InlineData("querykey.runtime.loader.ext.v10.snapshot.sda.sdasd.sdasd.asda.sdasd.as.das.d.dfsdf")]
    [InlineData("querykey.alpha.beta.gamma.delta.v1000.build")]
    [InlineData("eventkey.PREFIX.MY.PLUGIN.ID.v33.FINAL")]
    [InlineData("eventkey.service.dispatch.router.v7.patch")]
    [InlineData("eventkey.long.prefix.with.many.parts.v1234.a.a.a")]
    public void MessageKey_PatternValidation_Should_Succeed(string key)
    {
        Assert.True(MessageKeyValidator.ValidateKeyPattern(key));
    }

    [Theory]
    // Wrong plugin ID completely
    [InlineData("enginekey.DifferentPanel.Core.v12.enabled")]
    // Missing parts of the expected plugin ID
    [InlineData("querykey.LunaticPanel.Core.v2.BETA")]
    // Extra path segment injected into the middle of the ID
    [InlineData("eventkey.LunaticPanel.Core.Extra.Tests.MessageBus.v33.FINAL")]
    // Version positioned inside the plugin ID path
    [InlineData("enginekey.LunaticPanel.v12.Core.Tests.MessageBus.enabled")]
    public void MessageKey_PluginIdAssemblyValidation_Should_Fail(string key)
    {
        // Will return false because structural paths do not match, ignoring case.
        Assert.False(MessageKeyValidator.ValidateAssembly(key, "LunaticPanel.core.Tests.MessageBus"));
    }

    [Theory]
    // Completely invalid prefixes
    [InlineData("wrongkey.core.v12.enabled")]
    [InlineData("commandkey.alpha.beta.v1000.build")]
    // Valid prefix word but missing the required trailing dot
    [InlineData("enginekeycore.v12.enabled")]
    // Prefix is present but not at the very beginning
    [InlineData("invalid.enginekey.core.v12.enabled")]
    public void MessageKey_AllowedPrefixValidation_Should_Fail(string key)
    {
        Assert.False(MessageKeyValidator.ValidatePrefix(key));
    }

    [Theory]
    // Contains forbidden spaces
    [InlineData("enginekey.x.y.z.q .r.v992.release")]
    // Contains forbidden underscores
    [InlineData("querykey.PREFIX_MY.PLUGIN.ID.v2.BETA")]
    // Contains forbidden hyphens
    [InlineData("eventkey.service.dispatch-router.v7.patch")]
    // Contains forbidden special symbols
    [InlineData("querykey.alpha.beta$.v1000.build")]
    public void MessageKey_AllowedCharactersValidation_Should_Fail(string key)
    {
        Assert.False(MessageKeyValidator.ValidateAllowedCharacters(key));
    }

    [Theory]
    // Missing the numeric digit after 'v' (.v. instead of .v12.)
    [InlineData("enginekey.core.v.enabled")]
    // Version number starts with a forbidden zero (.v01.)
    [InlineData("querykey.alpha.beta.v01.build")]
    // Missing the trailing dot after the version number
    [InlineData("eventkey.service.dispatch.router.v7patch")]
    // Missing the version block completely
    [InlineData("enginekey.core.enabled")]
    // Missing the suffix component after the version block
    [InlineData("querykey.alpha.beta.gamma.delta.v1000.")]
    public void MessageKey_PatternValidation_Should_Fail(string key)
    {
        Assert.False(MessageKeyValidator.ValidateKeyPattern(key));
    }
    [Theory]
    // Contains forbidden spaces
    [InlineData("enginekey.x.y.z.q .r.v992.release")]
    // Contains forbidden underscores
    [InlineData("querykey.PREFIX_MY.PLUGIN.ID.v2.BETA")]
    // Contains forbidden hyphens
    [InlineData("eventkey.service.dispatch-router.v7.patch")]
    // Contains forbidden special symbols
    [InlineData("querykey.alpha.beta$.v1000.build")]
    public void Constructor_ShouldThrow_BusIdSchemticAllowedCharactersViolationException_WhenCharactersAreInvalid(string invalidKey)
    {
        // Act & Assert
        Assert.Throws<BusKeySchemticAllowedCharactersViolationException>(() => new MessageKey(invalidKey));
    }

    [Theory]
    // Completely invalid prefixes
    [InlineData("wrongkey.core.v12.enabled")]
    [InlineData("commandkey.alpha.beta.v1000.build")]
    // Valid prefix word but missing the required trailing dot
    [InlineData("enginekeycore.v12.enabled")]
    // Prefix is present but not at the very beginning
    [InlineData("invalid.enginekey.core.v12.enabled")]
    public void Constructor_ShouldThrow_BusIdSchemticPrefixViolationException_WhenPrefixIsInvalid(string invalidPrefixKey)
    {
        // Act & Assert
        Assert.Throws<BusKeySchemticPrefixViolationException>(() => new MessageKey(invalidPrefixKey));
    }

    [Theory]
    // Missing the numeric digit after 'v' (.v. instead of .v12.)
    [InlineData("enginekey.core.v.enabled")]
    // Version number starts with a forbidden zero (.v01.)
    [InlineData("querykey.alpha.beta.v01.build")]
    // Missing the trailing dot after the version number
    [InlineData("eventkey.service.dispatch.router.v7patch")]
    // Missing the version block completely
    [InlineData("enginekey.core.enabled")]
    // Missing the suffix component after the version block
    [InlineData("querykey.alpha.beta.gamma.delta.v1000.")]
    public void Constructor_ShouldThrow_BusIdSchematicPatternViolationException_WhenPatternIsInvalid(string invalidPatternKey)
    {
        // Act & Assert
        Assert.Throws<BusKeySchematicPatternViolationException>(() => new MessageKey(invalidPatternKey));
    }

    [Fact]
    public void OverloadedConstructor_ShouldThrow_WhenCombinedStringIsInvalid()
    {
        // Act & Assert
        Assert.Throws<BusKeySchemticAllowedCharactersViolationException>(() =>
            new MessageKey("enginekey", "invalid_plugin", "v12.enabled"));

        Assert.Throws<BusKeySchemticPrefixViolationException>(() =>
            new MessageKey("", () => "wrongkey.plugin", "v12.enabled"));
    }

    [Theory]
    [InlineData("enginekey.core.v12.enabled")]
    [InlineData("querykey.PREFIX.MY.PLUGIN.ID.v2.BETA")]
    [InlineData("querykey.runtime.loader.ext.v10.snapshot.sda.sdasd.sdasd.asda.sdasd.as.das.d.dfsdf")]
    [InlineData("querykey.alpha.beta.gamma.delta.v1000.build")]
    [InlineData("eventkey.PREFIX.MY.PLUGIN.ID.v33.FINAL")]
    [InlineData("eventkey.service.dispatch.router.v7.patch")]
    [InlineData("eventkey.long.prefix.with.many.parts.v1234.a.a.a")]
    public void Constructor_ShouldNotThrow_WhenStringIsValid(string validKey)
    {
        // Act
        var exception = Record.Exception(() => new MessageKey(validKey));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void OverloadedConstructors_ShouldNotThrow_WhenCombinedPartsAreValid()
    {
        // Act
        var exception1 = Record.Exception(() =>
            new MessageKey("enginekey", "LunaticPanel.Core", "v12.enabled"));

        var exception2 = Record.Exception(() =>
            new MessageKey("querykey", () => "alpha.beta.gamma.delta", "v1000.build"));

        // Assert
        Assert.Null(exception1);
        Assert.Null(exception2);
    }

    [Theory]
    [InlineData("enginekey.core.v12.enabled")]
    [InlineData("QUERYKEY.alpha.beta.v1.build")]
    public void ToString_ShouldReturn_LoweredFullname(string inputKey)
    {
        // Act
        var messageKey = new MessageKey(inputKey);

        // Assert
        Assert.Equal(inputKey.ToLower(), messageKey.ToString());
    }
}
