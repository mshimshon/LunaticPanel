using LuncaticPanel.Package.Server.Application.Payloads.Requests;
using LuncaticPanel.Package.Server.Application.Payloads.Responses;
using LuncaticPanel.Package.Server.Domain.Query;

namespace LuncaticPanel.Package.Server.Application.Payloads.Mapping;

public static class SearchMappingExt
{
    public static ManifestQueryModel ToDomain(this ManifestSearchRequest data)
    => new ManifestQueryModel()
    {
        Id = !string.IsNullOrWhiteSpace(data.PackageId) ? new(data.PackageId) : default,
        PanelVersion = !string.IsNullOrWhiteSpace(data.PanelVersion) ? new(data.PanelVersion) : default,
        Keywords = !string.IsNullOrWhiteSpace(data.Keywords) ? new(data.Keywords) : default,
        MaxResult = data.MaxResult,
        ShowEndOfLife = data.ShowEndOfLife,
        ShowHidden = data.ShowHidden,
        Position = data.Position
    };
    public static ManifestSearchResponse ToApplication(this IManifestQueryResultModel data)
        => new()
        {
            Position = data.Position,
            TotalResults = data.Total,
            Result = data.Result.Select(p => p.ToApplication()).ToList()
        };
}
