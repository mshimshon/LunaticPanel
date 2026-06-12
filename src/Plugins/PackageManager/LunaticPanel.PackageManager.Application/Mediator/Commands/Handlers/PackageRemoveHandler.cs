using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageRemoveHandler : IRequestHandler<PackageRemoveCommand>
{
    private readonly IPackageRepository _packageRepository;

    public PackageRemoveHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }
    public async Task Handle(PackageRemoveCommand command, CancellationToken ct = default)
    {
        try
        {
            PackageId id = new(command.Id);
            await _packageRepository.DisableAsync(id, ct);
            await _packageRepository.DeleteAsync(id, ct);
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {
            throw new HostUnkownException();
        }
    }
}
