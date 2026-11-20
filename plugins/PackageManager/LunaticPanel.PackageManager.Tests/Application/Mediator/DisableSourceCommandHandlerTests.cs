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

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class DisableSourceCommandHandlerTests
{
    private readonly ICrazyReport<DisableSourceCommandHandler> _crazyReport;
    private readonly ISourceRepository _sourceRepository;
    private readonly DisableSourceCommandHandler _handler;
    public DisableSourceCommandHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<DisableSourceCommandHandler>>();
        _sourceRepository = Substitute.For<ISourceRepository>();
        _handler = new DisableSourceCommandHandler(_sourceRepository, _crazyReport);
    }

    [Fact]
    public async Task DisableSourceCommand_ShouldDisable()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Enabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };

        await _handler.HandleAsync(new DisableSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await _sourceRepository.Received(1).DisableAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task DisableSourceCommand_ShouldRethrow()
    {
        RepositorySourcePayload repositorySourcePayload = new RepositorySourcePayload()
        {
            Name = "Source",
            State = RepositorySourceStatePayload.Enabled,
            Source = "",
            SourceType = RepositorySourceTypePayload.Local
        };
        _sourceRepository
            .DisableAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));

        var handler = _handler.HandleAsync(new DisableSourceCommand(repositorySourcePayload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
