using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Abstraction.Tools;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Application.Services;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;
using MaksimShimshon.Mediator;
using NSubstitute;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class MoveDownSourceCommandHandlerTests
{
    private readonly ICrazyReport<MoveDownSourceCommandHandler> _crazyReport;
    private readonly ISourceOrderingService _sourceOrderingService;
    private readonly MoveDownSourceCommandHandler _handler;
    public MoveDownSourceCommandHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<MoveDownSourceCommandHandler>>();
        _sourceOrderingService = Substitute.For<ISourceOrderingService>();
        _handler = new MoveDownSourceCommandHandler(_sourceOrderingService, _crazyReport);
    }

    [Fact]
    public async Task MoveDownSourceCommand_ShouldPass()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Disabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };

        await _handler.HandleAsync(new MoveDownSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await _sourceOrderingService.Received(1).MoveDownAsync(Arg.Any<RepositorySourcePayload>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveDownSourceCommand_ShouldRethrow()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Disabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };
        _sourceOrderingService
            .MoveDownAsync(Arg.Any<RepositorySourcePayload>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));

        var handler = _handler.HandleAsync(new MoveDownSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
