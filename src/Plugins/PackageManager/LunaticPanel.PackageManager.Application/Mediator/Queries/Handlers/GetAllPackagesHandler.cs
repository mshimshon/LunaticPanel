using LunaticPanel.Core.Abstraction.Widgets.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;

internal class GetAllPackagesHandler : IRequestHandler<GetAllPackagesQuery, ICollection<PackagePayload>>
{
    private readonly IPackageRepository _packageRepository;

    public GetAllPackagesHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }
    public async Task<ICollection<PackagePayload>> Handle(
        GetAllPackagesQuery query,
        CancellationToken ct = default)
    {
        try
        {
            ICollection<Domain.Entites.PackageEntity> result = await _packageRepository.GetAll(ct);

            return result.Select(p => p.ToApplicationPayload()).ToList();
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }

    }
}
