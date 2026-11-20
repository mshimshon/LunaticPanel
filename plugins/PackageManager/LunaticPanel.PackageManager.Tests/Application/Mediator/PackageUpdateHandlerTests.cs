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
using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using MaksimShimshon.Mediator;
using NSubstitute;
using StatePulse.Net;
using System;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class PackageUpdateHandlerTests
{
    private readonly IPackageDownloader _packageDownloader;
    private readonly IPackageRepository _packageRepository;
    private readonly IPackageFileReader _packageFileReader;
    private readonly PackageUpdateHandler _handler;
    public PackageUpdateHandlerTests()
    {
        _packageDownloader = Substitute.For<IPackageDownloader>();
        _packageRepository = Substitute.For<IPackageRepository>();
        _packageFileReader = Substitute.For<IPackageFileReader>();
        _handler = new PackageUpdateHandler(_packageFileReader, _packageDownloader, _packageRepository);
    }

    [Fact]
    public async Task PackageUpdateCommand_ShouldPass()
    {
        var item = PackageExt.CreatePackageManifest("dss.sdsd.dss", "Package Title", "1.0.0-preview.1", "2.2.0");
        var packagePayload = PackageExt.CreatePackage("dss.sdsd.dss", "Package Title", "1.0.0-preview.1", "2.2.0");
        _packageFileReader.GetPackageInformation(Arg.Any<string>()).Returns(packagePayload);
        await _handler.HandleAsync(new PackageUpdateCommand(item), TestContext.Current.CancellationToken);
        await _packageDownloader.Received().DownloadAsync(Arg.Any<PackageManifestPayload>(), Arg.Any<CancellationToken>());
        _packageFileReader.Received().GetPackageInformation(Arg.Any<string>());
        await _packageRepository.Received().UpdateAsync(Arg.Any<PackageEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PackageUpdateCommand_ShouldRethrow()
    {
        _packageRepository
            .InstallAsync(Arg.Any<PackageEntity>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));
        var item = PackageExt.CreatePackageManifest("dss.sdsd.dss", "Package Title", "1.0.0-preview.1", "2.2.0");
        var handler = _handler.HandleAsync(new PackageUpdateCommand(item), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
