using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Application.Payloads.Mapping;
using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Domain.QueryCriterias;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Mediator.Queries.Handlers;


public class SearchPackagesHandler
{
    public async Task<SearchResponse<PackageInfoPayload>> Handle(
        SearchPackageQuery query,
        IPackageRepository packageRepository,
        CancellationToken ct = default)
    {
        var queryModel = new PackageQueryModel();
        queryModel.SearchByKeywords(query.Keywords);
        IQueryModelResult<Domain.Entites.ValueObjects.PackageInfo> result = await packageRepository.QueryAsync(queryModel, ct);
        return result.ToApplicationSearchResponse(p => p.ToApplicationPayload());
    }
}
