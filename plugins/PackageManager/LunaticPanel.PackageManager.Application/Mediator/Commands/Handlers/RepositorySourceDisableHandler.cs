using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class RepositorySourceDisableHandler : IRequestHandler<RepositorySourceDisableCommand>
{
    private readonly ISourceRepository _sourceRepository;

    public RepositorySourceDisableHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }
    public async Task Handle(RepositorySourceDisableCommand command, CancellationToken ct = default)
    {
        try
        {
            var source = command.Source.ToDomainEntity();
            await _sourceRepository.EnableAsync(source, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
