using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class RepositorySourceRemoveHandler : IRequestHandler<RepositorySourceRemoveCommand>
{
    private readonly ISourceRepository _sourceRepository;

    public RepositorySourceRemoveHandler(ISourceRepository sourceRepository)
    {
        _sourceRepository = sourceRepository;
    }
    public async Task Handle(RepositorySourceRemoveCommand command, CancellationToken ct = default)
    {
        try
        {
            var source = command.Source.ToDomainEntity();
            await _sourceRepository.RemoveAsync(source, ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
