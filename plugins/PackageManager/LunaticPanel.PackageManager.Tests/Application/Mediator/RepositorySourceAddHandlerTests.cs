using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;

namespace LunaticPanel.PackageManager.Tests.Application.Mediator;

public class RepositorySourceAddHandlerTests
{
    private readonly ISourceRepository _sourceRepository;
    private readonly RepositorySourceAddHandler _handler;
    public RepositorySourceAddHandlerTests()
    {
        _sourceRepository = Substitute.For<ISourceRepository>();
        _handler = new RepositorySourceAddHandler(_sourceRepository);
    }

    [Fact]
    public async Task RepositorySourceAddCommand_ShouldPass()
    {

        var payload = PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled);
        await _handler.HandleAsync(new RepositorySourceAddCommand(payload), TestContext.Current.CancellationToken);
        await _sourceRepository.Received().AddAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RepositorySourceAddCommand_ShouldRethrow()
    {
        var payload = PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled);
        _sourceRepository
            .AddAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>())
            .Returns(_ => throw new Exception("Boom"));
        var handler = _handler.HandleAsync(new RepositorySourceAddCommand(payload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
