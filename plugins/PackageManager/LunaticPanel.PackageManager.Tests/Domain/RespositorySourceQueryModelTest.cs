using LunaticPanel.PackageManager.Domain.Entities.Enums;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Domain;

public class RespositorySourceQueryModelTest
{
    [Theory]
    [InlineData(0, RespositorySourceQueryModel.DEFAULT_MAX_RESULT, RespositorySourceQueryModel.DEFAULT_MAX_RESULT)]
    [InlineData(2, 5, RespositorySourceQueryModel.MIN_FLOORING_RESULT)]
    [InlineData(3, 51, RespositorySourceQueryModel.MAX_CEILING_RESULT)]
    public void PositionMaxResult_MapCorrectly(int position, int maxResult, int expectedMax)
    {

        var packageQueryModel = new RespositorySourceQueryModel(position, maxResult);
        Assert.True(packageQueryModel.Position == position);
        Assert.True(packageQueryModel.MaxResult == expectedMax);
    }


    [Theory]
    [InlineData(1, 1)]
    [InlineData(100, 100)]
    [InlineData(-1, 0)]
    public void Position_MapCorrectly(int position, int positionExpect)
    {

        var packageQueryModel = new RespositorySourceQueryModel(position);
        Assert.True(packageQueryModel.Position == positionExpect);
    }

    [Theory]
    [InlineData("first second", 2)]
    [InlineData("first second first", 3)]
    [InlineData("first second first last", 4)]
    public void Keyword_SplitCorrectly(string keywords, int expectedCount)
    {
        var packageQueryModel = new RespositorySourceQueryModel();
        packageQueryModel.SearchByKeywords(keywords);
        var correctCount = packageQueryModel.Keywords.Count == expectedCount;
        Assert.True(correctCount);
    }


    [Fact]
    public void FilterBySources_MapsCorrectly()
    {
        RespositorySourceQueryModel respositorySourceQueryModel = new RespositorySourceQueryModel();
        RepositorySourceType[] sourceTypes = [RepositorySourceType.Local, RepositorySourceType.Remote];
        respositorySourceQueryModel.FilterBySourceType(sourceTypes);
        Assert.Collection(respositorySourceQueryModel.RepositorySourceTypes,
            p => Assert.True(p == RepositorySourceType.Local),
            p => Assert.True(p == RepositorySourceType.Remote)
        );
    }
}
