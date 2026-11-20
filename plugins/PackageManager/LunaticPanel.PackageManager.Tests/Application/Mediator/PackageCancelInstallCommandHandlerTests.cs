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
using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using MaksimShimshon.Mediator;
using NSubstitute;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class PackageCancelInstallCommandHandlerTests
{
    private readonly ICrazyReport<PackageCancelInstallCommandHandler> _crazyReport;
    private readonly IHostDiskPackageService _hostDiskPackageService;
    private readonly PackageCancelInstallCommandHandler _handler;
    public PackageCancelInstallCommandHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<PackageCancelInstallCommandHandler>>();
        _hostDiskPackageService = Substitute.For<IHostDiskPackageService>();
        _handler = new PackageCancelInstallCommandHandler(_hostDiskPackageService, _crazyReport);
    }

    [Fact]
    public async Task CancelInstallPackageCommand_ShouldPass()
    {
        PackagePayload packagePayload = PackageExt.CreatePackage("panel.auth.azure", "Azure Authentication", "2.4.0", "5.1.0");
        await _handler.HandleAsync(new PackageCancelInstallCommand(packagePayload), TestContext.Current.CancellationToken);
        await _hostDiskPackageService.Received(1).CancelPendingUpdate(Arg.Any<PackagePayload>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task CancelInstallPackageCommand_ShouldRethrow()
    {
        PackagePayload packagePayload = PackageExt.CreatePackage("panel.auth.azure", "Azure Authentication", "2.4.0", "5.1.0");
        _hostDiskPackageService
            .CancelPendingUpdate(Arg.Any<PackagePayload>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));
        var handler = _handler.HandleAsync(new PackageCancelInstallCommand(packagePayload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
