using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.Exceptions;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Enums;
using LunaticPanel.PackageManager.Domain.Respositories.Exceptions;
using System;
using System.Runtime;
using System.Security.Cryptography.X509Certificates;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Domain;

public class PackageValueObjectTests
{

    [Theory]
    [InlineData("1.0.0")]
    [InlineData("11.0.0")]
    [InlineData("2.0.0-alpha.1")]
    public void PackagePanelVersion_Validate(string version)
    {
        var vo = new PackagePanelVersion(version);
        Assert.True(vo != default);
    }

    [Theory]
    [InlineData(1, 2, 0, "1.2.0")]
    [InlineData(1, 0, 1, "1.0.1")]
    [InlineData(1, null, null, "1.0.0")]
    [InlineData(1, 0, null, "1.0.0")]
    public void PackagePanelVersion_ValidSemVerSimple(int major, int? minor, int? patch, string expected)
    {
        string result;
        if (minor == null)
            result = new PackagePanelVersion(major).Value;
        else if (patch == null)
            result = new PackagePanelVersion(major, (int)minor).Value;
        else
            result = new PackagePanelVersion(major, (int)minor, (int)patch).Value;

        Assert.True(result == expected);
    }


    [Theory]
    [InlineData("1.0.0-alpha")]
    [InlineData("2.0.0-alpha+build.222")]
    [InlineData("2.0.0-alpha.1+build")]
    public void PackagePanelVersion_PackageVersionInvalidException(string version)
    {
        Assert.Throws<PackageVersionInvalidException>(() => new PackagePanelVersion(version));
    }
    [Theory]
    [InlineData("1.0.0")]
    [InlineData("11.0.0")]
    [InlineData("2.0.0-alpha.1")]
    public void PackageVersion_Validate(string version)
    {
        var vo = new PackageVersion(version);
        Assert.True(vo != default);
    }

    [Theory]
    [InlineData(1, 2, 0, "1.2.0")]
    [InlineData(1, 0, 1, "1.0.1")]
    [InlineData(1, null, null, "1.0.0")]
    [InlineData(1, 0, null, "1.0.0")]
    public void PackageVersion_ValidSemVerSimple(int major, int? minor, int? patch, string expected)
    {
        string result;
        if (minor == null)
            result = new PackageVersion(major).Value;
        else if (patch == null)
            result = new PackageVersion(major, (int)minor).Value;
        else
            result = new PackageVersion(major, (int)minor, (int)patch).Value;

        Assert.True(result == expected);
    }


    [Theory]
    [InlineData("1.0.0-alpha")]
    [InlineData("2.0.0-alpha+build.222")]
    [InlineData("2.0.0-alpha.1+build")]
    public async Task PackageVersion_PackageVersionInvalidException(string version)
    {
        await Assert.ThrowsAsync<PackageVersionInvalidException>(() => Task.FromResult(new PackageVersion(version)));
    }


    [Theory]
    [InlineData("dsdas.dsdas.dasd")]
    [InlineData("sdfsdf.dfsdf.sdf")]
    [InlineData("fdg.fgdfg.dfgdfg.dfs")]
    public async Task PackageNotFoundException_Success(string packageId)
    {
        await Assert.ThrowsAsync<PackageNotFoundException>(() => throw new PackageNotFoundException(new PackageId(packageId)));
    }

    [Fact]
    public void PackageFailure_Success()
    {
        var item = new PackageFailure("Error");
        Assert.True(item.Value == "Error");
    }

    [Theory]
    [InlineData("Mad Maks", "/tmp/plugins", RepositorySourceState.Enabled, RepositorySourceType.Local, "a failure")]
    [InlineData("Cool Plugin", "/var/lib/plugins", RepositorySourceState.Enabled, RepositorySourceType.Local, null)]
    [InlineData("Remote Mod", "https://example.com", RepositorySourceState.Enabled, RepositorySourceType.Remote, "a failure")]
    [InlineData("Beta Tool", "https://example.com", RepositorySourceState.Enabled, RepositorySourceType.Remote, null)]
    public void RepositorySourceEntity_Success(
        string nameStr,
        string sourceLocation,
        RepositorySourceState state,
        RepositorySourceType sourceType,
        string? failureMessage)
    {
        var name = new RepositorySourceName(nameStr);
        var sourceInfo = sourceType == RepositorySourceType.Local
            ? new RepositorySourceInfo(new RepositorySourceLocal(sourceLocation), sourceType)
            : new RepositorySourceInfo(new RepositorySourceRemote(sourceLocation), sourceType);
        var failure = failureMessage != default ? new RepositoryFailure(failureMessage) : default;
        RepositorySourceEntity? item = default;
        if (failure != default)
            item = new RepositorySourceEntity(name, sourceInfo, failure, state);
        else
            item = new RepositorySourceEntity(name, sourceInfo, state);


        Assert.Equal(nameStr, item.Name.Value);
        Assert.Equal(sourceLocation, item.Info.Source.Value);
        Assert.Equal(sourceType, item.Info.SourceType);
        Assert.Equal(state, item.State);

        if (failureMessage != default)
            Assert.Equal(failureMessage, item.Failure!.Value);

    }

    [Theory]
    [InlineData("aaaa.aaaa.aaaa", "Mad Maks", "My description of Package", "/tmp/plugins", "1.0.0", "1.0.0", RepositorySourceType.Local, "a failure")]
    [InlineData("bbbb.bbbb.bbbb", "Cool Plugin", "Another package description", "/var/lib/plugins", "2.1.3", "1.2.0", RepositorySourceType.Local, null)]
    [InlineData("cccc.cccc.cccc", "Remote Mod", "Fetched from remote repo", "https://example.com", "1.0.0-alpha.1", "1.0.0-alpha.1", RepositorySourceType.Remote, "a failure")]
    [InlineData("dddd.dddd.dddd", "Beta Tool", "Pre-release testing tool", "https://example.com", "3.4.2-beta.12", "2.0.0-rc.4", RepositorySourceType.Remote, null)]
    public void PackageEntity_Success(
        string idStr,
        string nameStr,
        string descStr,
        string sourceLocation,
        string versionStr,
        string panelVersionStr,
        RepositorySourceType sourceType,
        string? failureMessage)
    {
        var states = Enum.GetValues<PackageState>();
        var randomState = states[Random.Shared.Next(states.Length)];

        var id = new PackageId(idStr);
        var name = new PackageName(nameStr);
        var desc = new PackageDescription(descStr);
        var info = new PackageInfo(id, name, desc, randomState);

        var sourceInfo = sourceType == RepositorySourceType.Local
            ? new RepositorySourceInfo(new RepositorySourceLocal(sourceLocation), sourceType)
            : new RepositorySourceInfo(new RepositorySourceRemote(sourceLocation), sourceType);

        var version = new PackageVersion(versionStr);
        var panelVersion = new PackagePanelVersion(panelVersionStr);
        var dependency = new List<PackageDependency>();
        var failure = failureMessage != default ? new PackageFailure(failureMessage) : default;

        PackageEntity? item = default;
        if (failure != default)
            item = new PackageEntity(info, sourceInfo, version, panelVersion, dependency, failure);
        else
            item = new PackageEntity(info, sourceInfo, version, panelVersion, dependency);

        Assert.Equal(idStr, item.Info.Id.Value);
        Assert.Equal(nameStr, item.Info.Name.Value);
        Assert.Equal(descStr, item.Info.Description.Value);
        Assert.Equal(randomState, item.Info.State);
        Assert.Equal(sourceLocation, item.Source.Source.Value);

        if (failureMessage != default)
            Assert.Equal(failureMessage, item.Failure!.Value);

        Assert.Equal(versionStr, item.Version.Value);
        Assert.Equal(panelVersionStr, item.PanelVersion.Value);
        Assert.Equal(sourceType, item.Source.SourceType);
    }
}
