using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.Core.Utils.Abstraction.Logging;
using LunaticPanel.PackageManager.Application.Mediator.Commands;
using LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;
using LunaticPanel.PackageManager.Application.Payloads.Enums;
using LunaticPanel.PackageManager.Pulses.States;
using LunaticPanel.PackageManager.Domain.Entities;
using LunaticPanel.PackageManager.Domain.Respositories;

using LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator.Extensions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace LunaticPanel.PackageManager....LunaticPanel.PackageManager.Tests.Application.Mediator;

public class RepositorySourceRemoveHandlerTests
{
    private readonly ICrazyReport<RepositorySourceRemoveHandler> _crazyReport;
    private readonly ISourceRepository _sourceRepository;
    private readonly RepositorySourceRemoveHandler _handler;
    public RepositorySourceRemoveHandlerTests()
    {
        _crazyReport = Substitute.For<ICrazyReport<RepositorySourceRemoveHandler>>();
        _sourceRepository = Substitute.For<ISourceRepository>();
        _handler = new RepositorySourceRemoveHandler(_sourceRepository, _crazyReport);
    }

    [Fact]
    public async Task RepositorySourceRemoveCommand_ShouldPass()
    {

        var payload = PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled);
        await _handler.HandleAsync(new RepositorySourceRemoveCommand(payload), TestContext.Current.CancellationToken);
        await _sourceRepository.Received().RemoveAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RepositorySourceRemoveCommand_ShouldRethrow()
    {
        var payload = PackageExt.CreatePackageSource("Source Name", "/tmp/sources", RepositorySourceTypePayload.Local, RepositorySourceStatePayload.Enabled);
        _sourceRepository
            .RemoveAsync(Arg.Any<RepositorySourceEntity>(), Arg.Any<CancellationToken>())
            .Throws<Exception>();
        var handler = _handler.HandleAsync(new RepositorySourceRemoveCommand(payload), TestContext.Current.CancellationToken);
        await Assert.ThrowsAsync<HostUnkownException>(() => handler);
    }
}
