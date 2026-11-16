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

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class PackageEnableHandlerTests
{
    private readonly IPackageRepository _packageRepository;
    private readonly PackageEnableHandler _handler;
    public PackageEnableHandlerTests()
    {
        _packageRepository = Substitute.For<IPackageRepository>();
        _handler = new PackageEnableHandler(_packageRepository);
    }

    [Fact]
    public async Task PackageEnableCommand_ShouldPass()
    {
        await _handler.HandleAsync(new PackageEnableCommand("panel.auth.azure"), TestContext.Current.CancellationToken);
        await _packageRepository.Received(1).EnableAsync(Arg.Any<PackageId>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PackageEnableCommand_ShouldRethrow()
    {
        _packageRepository
            .EnableAsync(Arg.Any<PackageId>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));
        var handler = _handler.HandleAsync(new PackageEnableCommand("panel.auth.azure"), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
