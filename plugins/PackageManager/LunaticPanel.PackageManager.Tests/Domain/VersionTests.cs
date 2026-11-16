using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.Exceptions;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryCriterias;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Enums;
using LunaticPanel.PackageManager.Domain.Respositories.Exceptions;
using NuGet.Versioning;
using System;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;

namespace LunaticPanel.PackageManager.Tests.Domain;

public class VersionTests
{

    [Theory]
    // 1. Standard Major, Minor, and Patch increments
    [InlineData("2.0.0", "1.0.0")]
    [InlineData("1.1.0", "1.0.0")]
    [InlineData("1.0.1", "1.0.0")]
    [InlineData("1.2.0", "1.1.9")]
    [InlineData("2.0.0", "1.9.9")]
    [InlineData("10.0.0", "2.0.0")]
    [InlineData("1.10.0", "1.2.0")]
    [InlineData("1.0.10", "1.0.2")]

    // 2. Normal version vs. Pre-release version (Normal > Pre-release)
    [InlineData("1.0.0", "1.0.0-alpha")]
    [InlineData("1.0.0", "1.0.0-rc.1")]
    [InlineData("1.0.0", "1.0.0-0.3.7")]
    [InlineData("1.0.1", "1.0.1-beta")]
    [InlineData("1.0.0-beta", "1.0.0-alpha")]
    [InlineData("1.0.0-alpha.beta", "1.0.0-alpha.1")]
    [InlineData("1.0.0-beta.2", "1.0.0-beta.1")]
    [InlineData("1.0.0-beta.11", "1.0.0-beta.2")]
    [InlineData("1.0.0-rc.1", "1.0.0-beta.11")]
    [InlineData("1.0.0-alpha.1", "1.0.0-alpha")]
    [InlineData("1.0.0-1", "1.0.0-0")]
    [InlineData("1.0.0-alpha", "1.0.0-1")]

    [InlineData("1.0.1+build.1", "1.0.0+build.999")]
    [InlineData("1.0.0+20130313144700", "1.0.0-alpha+exp.sha.5114f85")]
    [InlineData("1.0.0-beta+exp.sha", "1.0.0-alpha+20130313144700")]
    public void NuGetVersion_ValidateGreaterThan(string left, string right)
    {
        Assert.True(NuGetVersion.Parse(left) > NuGetVersion.Parse(right));
        Assert.False(NuGetVersion.Parse(left) < NuGetVersion.Parse(right));
        Assert.True(new NuGetVersion(left) > new NuGetVersion(right));
        Assert.True(new NuGetVersion(left) > NuGetVersion.Parse(right));
    }

}
