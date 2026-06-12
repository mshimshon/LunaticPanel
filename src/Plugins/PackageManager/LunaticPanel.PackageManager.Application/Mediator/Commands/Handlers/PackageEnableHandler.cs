using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Commands.Handlers;

internal class PackageEnableHandler : IRequestHandler<PackageEnableCommand>
{
    private readonly IPackageRepository _packageRepository;

    public PackageEnableHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }
    public async Task Handle(PackageEnableCommand command, CancellationToken ct = default)
    {
        try
        {
            await _packageRepository.EnableAsync(new(command.Id), ct);

            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }
    }
}
