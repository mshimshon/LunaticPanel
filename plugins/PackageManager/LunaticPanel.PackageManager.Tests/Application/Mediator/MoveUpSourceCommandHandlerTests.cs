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

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class MoveUpSourceCommandHandlerTests
{
    private readonly ICrazyReport<MoveUpSourceCommandHandler> _crazyReport;
    private readonly ISourceOrderingService _sourceOrderingService;
    private readonly MoveUpSourceCommandHandler _handler;
    public MoveUpSourceCommandHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<MoveUpSourceCommandHandler>>();
        _sourceOrderingService = Substitute.For<ISourceOrderingService>();
        _handler = new MoveUpSourceCommandHandler(_sourceOrderingService, _crazyReport);
    }

    [Fact]
    public async Task MoveUpSourceCommand_ShouldPass()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Disabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };

        await _handler.HandleAsync(new MoveUpSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await _sourceOrderingService.Received(1).MoveUpAsync(Arg.Any<RepositorySourcePayload>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task MoveUpSourceCommand_ShouldRethrow()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Disabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };
        _sourceOrderingService
            .MoveUpAsync(Arg.Any<RepositorySourcePayload>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));

        var handler = _handler.HandleAsync(new MoveUpSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
