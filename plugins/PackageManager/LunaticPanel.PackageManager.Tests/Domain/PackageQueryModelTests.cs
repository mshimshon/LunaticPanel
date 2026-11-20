using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Enums;
using System;

namespace LunaticPanel.PackageManager.Tests.Domain;

public class PackageQueryModelTests
{
    private RepositorySourceInfo CreateSourceInfo()
    {
        var source = new RepositorySourceLocal("dontcare");
        var sourceInfo = new RepositorySourceInfo(source, RepositorySourceType.Local);
        return sourceInfo;
    }

    [Fact]
    public void FilterBySources_MapsCorrectly()
    {
        PackageQueryModel packageQueryModel = new PackageQueryModel();
        RepositorySourceInfo[] sources = [CreateSourceInfo()];
        packageQueryModel.FilterBySources(sources);
        Assert.Collection(packageQueryModel.RepositorySources, p => Assert.True(p.SourceType == RepositorySourceType.Local));
    }

    [Fact]
    public void FilterByStates_MapsCorrectly()
    {
        PackageQueryModel packageQueryModel = new PackageQueryModel();
        packageQueryModel.FilterByStates(PackageState.Enabled, PackageState.Unknown);
        Assert.Collection(packageQueryModel.PackageStates,
                p => Assert.True(p == PackageState.Enabled),
                p => Assert.True(p == PackageState.Unknown)
            );
    }

    [Fact]
    public void FilterByIds_MapsCorrectly()
    {
        PackageQueryModel packageQueryModel = new PackageQueryModel();
        packageQueryModel.FilterByIds(new PackageId("My.Id"));
        var foundID = packageQueryModel.PackageIds.Any(p => p.Value == "My.Id");
        Assert.True(foundID);
    }

    [Theory]
    [InlineData("first second", 2)]
    [InlineData("first second first", 3)]
    [InlineData("first second first last", 4)]
    public void Keyword_SplitCorrectly(string keywords, int expectedCount)
    {
        var packageQueryModel = new PackageQueryModel();
        packageQueryModel.SearchByKeywords(keywords);
        var correctCount = packageQueryModel.Keywords.Count == expectedCount;
        Assert.True(correctCount);
    }
    [Theory]
    [InlineData("1.0.0", ValueComparision.Exactly)]
    [InlineData("1.0.0", ValueComparision.GreaterThan)]
    [InlineData("2.0.0-alpha.1", ValueComparision.LowerThan)]
    public void Version_MapCorrectly(string version, ValueComparision comparision)
    {

        var packageQueryModel = new PackageQueryModel();
        var packageVersion = new PackageVersion(version);
        packageQueryModel.OnlyVersion(packageVersion, comparision);
        Assert.True(packageQueryModel.SpecificVersion == packageVersion);
        Assert.True(packageQueryModel.SpecificVersionComparision == comparision);
    }

    [Theory]
    [InlineData(0, PackageQueryModel.DEFAULT_MAX_RESULT, PackageQueryModel.DEFAULT_MAX_RESULT)]
    [InlineData(2, 5, PackageQueryModel.MIN_FLOORING_RESULT)]
    [InlineData(3, 51, PackageQueryModel.MAX_CEILING_RESULT)]
    public void PositionMaxResult_MapCorrectly(int position, int maxResult, int expectedMax)
    {

        var packageQueryModel = new PackageQueryModel(position, maxResult);
        Assert.True(packageQueryModel.Position == position);
        Assert.True(packageQueryModel.MaxResult == expectedMax);
    }


    [Theory]
    [InlineData(1, 1)]
    [InlineData(100, 100)]
    [InlineData(-1, 0)]
    public void Position_MapCorrectly(int position, int positionExpect)
    {

        var packageQueryModel = new PackageQueryModel(position);
        Assert.True(packageQueryModel.Position == positionExpect);
    }

    [Fact]
    public void EmptyFilter_MapCorrectly()
    {

        var packageQueryModel = new PackageQueryModel();
        Assert.True(packageQueryModel.SpecificVersion == default);
        Assert.True(packageQueryModel.SpecificVersionComparision == default);
        Assert.True(packageQueryModel.PackageIds.Count == 0);
        Assert.True(packageQueryModel.Keywords.Count == 0);
        Assert.True(packageQueryModel.PackageStates.Count == 0);
        Assert.True(packageQueryModel.RepositorySources.Count == 0);
        Assert.True(packageQueryModel.Position == 0);
        Assert.True(packageQueryModel.MaxResult == QueryModel.DEFAULT_MAX_RESULT);
    }
}
