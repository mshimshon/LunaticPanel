using LunaticPanel.PackageManager.Application.Payloads.Responses;
using LunaticPanel.PackageManager.Domain.QueryModels;
using LunaticPanel.PackageManager.Domain.QueryModels.Interfaces;

namespace LunaticPanel.PackageManager.Application.Payloads.Mapping;

public static class SearchMapping
{
    public static IQueryModelResult<TEntity> ToDomainQueryModel<TEntity, TPayload>(this SearchResponse<TPayload> data, Func<TPayload, TEntity> forEach)
        where TEntity : class
        where TPayload : class
        => new QueryModelResult<TEntity>()
        {
            Result = data.Result.Select(forEach).ToList(),
            Total = data.Total,
            Position = data.Position
        };

    public static SearchResponse<TPayload> ToApplicationSearchResponse<TEntity, TPayload>(this IQueryModelResult<TEntity> data, Func<TEntity, TPayload> forEach)
    where TEntity : class
    where TPayload : class
    => new SearchResponse<TPayload>()
    {
        Result = data.Result.Select(forEach).ToList(),
        Total = data.Total,
        Position = data.Position
    };
}
