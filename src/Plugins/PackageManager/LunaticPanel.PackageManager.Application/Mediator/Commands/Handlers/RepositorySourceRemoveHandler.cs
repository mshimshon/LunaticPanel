using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class RepositorySourceRemoveHandler
{
    public async Task Handle(RepositorySourceRemoveCommand command, ISourceRepository sourceRepository, CancellationToken ct = default)
    {
        try
        {
            var source = command.Source.ToDomainEntity();
            await sourceRepository.RemoveAsync(source, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
