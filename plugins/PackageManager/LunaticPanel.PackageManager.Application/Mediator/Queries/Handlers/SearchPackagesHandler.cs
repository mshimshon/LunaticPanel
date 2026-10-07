using LunaticPanel.Core.Abstraction.Exceptions;
using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Domain.QueryCriterias;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;
using MedihatR;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;


internal class SearchPackagesHandler : IRequestHandler<SearchPackageQuery, SearchResponse<PackageInfoPayload>>
{
    private readonly IPackageRepository _packageRepository;

    public SearchPackagesHandler(IPackageRepository packageRepository)
    {
        _packageRepository = packageRepository;
    }
    public async Task<SearchResponse<PackageInfoPayload>> Handle(
        SearchPackageQuery query,
        CancellationToken ct = default)
    {
        try
        {
            var queryModel = new PackageQueryModel();
            //TODO: FLUENT VALIDATION FOR QUERY
            queryModel.SearchByKeywords(query.Keywords);
            IQueryModelResult<Domain.Entities.ValueObjects.PackageInfo> result = await _packageRepository.QueryAsync(queryModel, ct);
            return result.ToApplicationSearchResponse(p => p.ToApplicationPayload());
            //TODO: HANDLE DOMAIN EXCEPTIONS
        }
        catch (Exception)
        {

            throw new HostUnkownException();
        }

    }
}
