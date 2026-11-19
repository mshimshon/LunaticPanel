using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class RepositorySourceEnableHandlerTests
{
    private readonly ISourceRepository _sourceRepository;
    private readonly RepositorySourceEnableHandler _handler;
    public RepositorySourceEnableHandlerTests()
    {
        _sourceRepository = Substitute.For<ISourceRepository>();
        _handler = new RepositorySourceEnableHandler(_sourceRepository);
    }

    [Fact]
    public async Task RepositorySourceEnableCommand_ShouldPass()
    {

        var payload = PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled);
        await _handler.HandleAsync(new RepositorySourceEnableCommand(payload), TestContext.Current.CancellationToken);
        await _sourceRepository.Received().EnableAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RepositorySourceEnableCommand_ShouldRethrow()
    {
        var payload = PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled);
        _sourceRepository
            .EnableAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));
        var handler = _handler.HandleAsync(new RepositorySourceEnableCommand(payload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
