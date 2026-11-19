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
using LunaticPanel.PackageManager.Domain.Entities.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;
using MaksimShimshon.Mediator;
using NSubstitute;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class PackageRemoveHandlerTests
{
    private readonly IPackageRepository _packageRepository;
    private readonly ICrazyReport<PackageRemoveHandler> _crazyReport;
    private readonly PackageRemoveHandler _handler;
    public PackageRemoveHandlerTests()
    {
        _packageRepository = Substitute.For<IPackageRepository>();
        _crazyReport = Substitute.For<ICrazyReport<PackageRemoveHandler>>();
        _handler = new PackageRemoveHandler(_packageRepository, _crazyReport);
    }

    [Fact]
    public async Task PackageRemoveCommand_ShouldPass()
    {
        await _handler.HandleAsync(new PackageRemoveCommand("panel.auth.azure"), TestContext.Current.CancellationToken);
        await _packageRepository.Received().DisableAsync(Arg.Any<PackageId>(), Arg.Any<CancellationToken>());
        await _packageRepository.Received().DeleteAsync(Arg.Any<PackageId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PackageRemoveCommand_ShouldRethrow()
    {
        _packageRepository
            .DisableAsync(Arg.Any<PackageId>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));
        var handler = _handler.HandleAsync(new PackageRemoveCommand("panel.auth.azure"), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
