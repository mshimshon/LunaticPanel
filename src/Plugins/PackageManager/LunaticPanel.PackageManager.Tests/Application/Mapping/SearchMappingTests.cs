using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;

namespace LunaticPanel.PackageManager.Tests.Application.Mapping;

public class SearchMappingTests
{
    public static PackageInfoPayload RepositoryMock = new PackageInfoPayload()
    {
        PackageId = "TEST.ID",
        Name = "TEST PACKAGE",
        Description = "DESC"
    };

    public static PackageInfo PackageMock { get; } =
        new PackageInfo(new("TEST.ID"), new("TEST PACKAGE"), new("DESC"));

    // -----------------------------
    // TEST: SearchResponse -> QueryModelResult
    // -----------------------------
    [Fact]
    public void ToDomainQueryModel_MapsCorrectly()
    {
        var response = new SearchResponse<PackageInfoPayload>
        {
            Result = new List<PackageInfoPayload> { RepositoryMock },
            Total = 1,
            Position = 100
        };
        var result = response.ToDomainQueryModel(p => p.ToDomainEntity());

        Assert.Single(result.Result);
        Assert.Equal(PackageMock, result.Result.First());
        Assert.Equal(1, result.Total);
        Assert.Equal(100, result.Position);
    }

    // -----------------------------
    // TEST: QueryModelResult -> SearchResponse
    // -----------------------------
    [Fact]
    public void ToApplicationSearchResponse_MapsCorrectly()
    {
        var response = new QueryModelResult<PackageInfo>
        {
            Result = new List<PackageInfo> { PackageMock },
            Total = 1,
            Position = 100
        };
        var result = response.ToApplicationSearchResponse(p => p.ToApplicationPayload());

        Assert.Single(result.Result);
        Assert.Equal(RepositoryMock, result.Result.First());
        Assert.Equal(1, result.Total);
        Assert.Equal(100, result.Position);
    }
}
