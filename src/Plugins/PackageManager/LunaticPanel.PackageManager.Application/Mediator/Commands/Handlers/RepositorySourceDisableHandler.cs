using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class RepositorySourceDisableHandler
{
    public async Task Handle(RepositorySourceDisableCommand command, ISourceRepository sourceRepository, CancellationToken ct = default)
    {
        try
        {
            var source = command.Source.ToDomainEntity();
            await sourceRepository.EnableAsync(source, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
