using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using MaksimShimshon.Mediator;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class RepositorySourceDisableHandler : IRequestHandler<RepositorySourceDisableCommand>
{
    private readonly ISourceRepository _sourceRepository;

    public RepositorySourceDisableHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }
    public async Task HandleAsync(RepositorySourceDisableCommand command, CancellationToken ct = default)
    {
        try
        {
            var source = command.Source.ToDomainEntity();
            await _sourceRepository.DisableAsync(source, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception ex)
        {
            throw new HostUnkownException();
        }
    }
}
