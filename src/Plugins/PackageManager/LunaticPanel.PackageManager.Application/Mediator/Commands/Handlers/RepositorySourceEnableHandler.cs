using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class RepositorySourceEnableHandler : IRequestHandler<RepositorySourceEnableCommand>
{
    private readonly ISourceRepository _sourceRepository;

    public RepositorySourceEnableHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }
    public async Task Handle(RepositorySourceEnableCommand command, CancellationToken ct = default)
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
