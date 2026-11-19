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
using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using MaksimShimshon.Mediator;
using NSubstitute;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class PackageRollbackCommandHandlerTests
{
    private readonly IHostDiskPackageService _hostDiskPackageService;
    private readonly ICrazyReport<PackageRollbackCommandHandler> _crazyReport;
    private readonly PackageRollbackCommandHandler _handler;
    public PackageRollbackCommandHandlerTests()
    {
        _hostDiskPackageService = Substitute.For<IHostDiskPackageService>();
        _crazyReport = Substitute.For<ICrazyReport<PackageRollbackCommandHandler>>();
        _handler = new PackageRollbackCommandHandler(_hostDiskPackageService, _crazyReport);
    }

    [Fact]
    public async Task PackageRollbackCommand_ShouldPass()
    {
        var packagePayload = PackageExt.CreatePackage("dss.sdsd.dss", "Package Title", "1.0.0-preview.1", "2.2.0");
        await _handler.HandleAsync(new PackageRollbackCommand(packagePayload), TestContext.Current.CancellationToken);
        await _hostDiskPackageService.Received().RollbackTo(Arg.Any<PackagePayload>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PackageRollbackCommand_ShouldRethrow()
    {
        var packagePayload = PackageExt.CreatePackage("dss.sdsd.dss", "Package Title", "1.0.0-preview.1", "2.2.0");
        _hostDiskPackageService
            .RollbackTo(Arg.Any<PackagePayload>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));
        var handler = _handler.HandleAsync(new PackageRollbackCommand(packagePayload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
