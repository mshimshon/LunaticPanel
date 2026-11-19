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
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;
using NSubstitute.Core.Arguments;
using System.Diagnostics.CodeAnalysis;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class GetSourcesHandlerTests
{
    private readonly ICrazyReport<GetSourcesHandler> _crazyReport;
    private readonly ISourceRepository _sourceRepository;
    private readonly GetSourcesHandler _handler;
    private readonly ICollection<RepositorySourceEntity> _packageSources =
      [
            PackageExt.CreateRepositorySourceEntity("Source A", "http://sss.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote, PackageManager.Domain.Entities.Enums.RepositorySourceState.Enabled),
            PackageExt.CreateRepositorySourceEntity("Source B", "http://sss3.com/", PackageManager.Domain.Entities.Enums.RepositorySourceType.Remote, PackageManager.Domain.Entities.Enums.RepositorySourceState.Disabled),
      ];

    public GetSourcesHandlerTests()
    {
        _sourceRepository = Substitute.For<ISourceRepository>();
        _crazyReport = Substitute.For<ICrazyReport<GetSourcesHandler>>();
        _sourceRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(_packageSources);
        _handler = new GetSourcesHandler(_sourceRepository, _crazyReport);
    }

    [Fact]
    public async Task GetSourcesQuery_ShouldPass()
    {
        ICollection<RepositorySourcePayload> expected = _packageSources.Select(p => p.ToApplicationPayload()).ToList();
        var result = await _handler.HandleAsync(new GetSourcesQuery(), TestContext.Current.CancellationToken);
        await _sourceRepository.Received().GetAllAsync(Arg.Any<CancellationToken>());
        Assert.Equivalent(expected, result);
    }

    private ICollection<RepositorySourceEntity> Throw() => throw new Exception("Boom");

    [Fact]
    public async Task GetSourcesQuery_ShouldRethrow()
    {
        _sourceRepository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(_ => Throw());
        var handler = _handler.HandleAsync(new GetSourcesQuery(), TestContext.Current.CancellationToken);
        await _sourceRepository.Received().GetAllAsync(Arg.Any<CancellationToken>());
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
