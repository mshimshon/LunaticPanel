using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using MaksimShimshon.Mediator;
using NSubstitute;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class EnableSourceCommandHandlerTests
{
    private readonly ICrazyReport<EnableSourceCommandHandler> _crazyReport;
    private readonly ISourceRepository _sourceRepository;
    private readonly EnableSourceCommandHandler _handler;
    public EnableSourceCommandHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<EnableSourceCommandHandler>>();
        _sourceRepository = Substitute.For<ISourceRepository>();
        _handler = new EnableSourceCommandHandler(_sourceRepository, _crazyReport);
    }

    [Fact]
    public async Task EnableSourceCommand_ShouldDisable()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Disabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };

        await _handler.HandleAsync(new EnableSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await _sourceRepository.Received(1).EnableAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task EnableSourceCommand_ShouldRethrow()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Disabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };
        _sourceRepository
            .EnableAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));

        var handler = _handler.HandleAsync(new EnableSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
