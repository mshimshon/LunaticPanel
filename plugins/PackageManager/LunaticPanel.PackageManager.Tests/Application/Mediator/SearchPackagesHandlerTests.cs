using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Mediator.Queries;
using LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;
using NSubstitute.Core.Arguments;
using NSubstitute.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class SearchPackagesHandlerTests
{
    private readonly ICrazyReport<SearchPackagesHandler> _crazyReport;
    private readonly IPackageRepository _packageRepository;
    private readonly SearchPackagesHandler _handler;
    private readonly ICollection<PackageEntity> _runtimePackage =
    [
        PackageExt.CreatePackageEntity("panel.analytics.reports", "Analytics Reports", "0.9.0",  "5.2.0-preview.1", PackageManager.Domain.Entities.Enums.PackageState.Enabled, "https://source.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote)
    ];
    public SearchPackagesHandlerTests()
    {
        _packageRepository = Substitute.For<IPackageRepository>();
        _crazyReport = Substitute.For<ICrazyReport<SearchPackagesHandler>>();
        _packageRepository.QueryAsync(Arg.Any<IPackageQueryModel>(), Arg.Any<CancellationToken>()).Returns(CallInfo =>
        {
            var result = _runtimePackage.Select(p => p.Info).ToList();
            return new QueryModelResult<PackageInfo>()
            {
                Result = result,
                Position = 0,
                Total = result.Count
            };
        });
        _handler = new SearchPackagesHandler(_packageRepository);
    }

    [Fact]
    public async Task GetSourcesQuery_ShouldPass()
    {
        ICollection<PackageInfoPayload> expected = _runtimePackage.Select(p => p.Info.ToApplicationPayload()).ToList();
        var result = await _handler.HandleAsync(new SearchPackageQuery() { Keywords = "wonderful" }, TestContext.Current.CancellationToken);
        await _packageRepository.Received().QueryAsync(Arg.Any<IPackageQueryModel>(), Arg.Any<CancellationToken>());
        Assert.Equivalent(expected, result.Result);
        Assert.Equal(0, result.Position);
        Assert.Equal(expected.Count, result.TotalResults);
    }

    private IQueryModelResult<PackageInfo> Throw() => throw new Exception("Boom");

    [Fact]
    public async Task GetSourcesQuery_ShouldRethrow()
    {
        _packageRepository.QueryAsync(Arg.Any<IPackageQueryModel>(), Arg.Any<CancellationToken>()).Returns(_ => Throw());
        var handler = _handler.HandleAsync(new SearchPackageQuery() { Keywords = "wonderful" }, TestContext.Current.CancellationToken);
        await _packageRepository.Received().QueryAsync(Arg.Any<IPackageQueryModel>(), Arg.Any<CancellationToken>());
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
