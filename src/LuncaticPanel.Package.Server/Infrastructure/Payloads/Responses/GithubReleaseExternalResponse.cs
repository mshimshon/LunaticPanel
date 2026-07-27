namespace LuncaticPanel.Package.Server.Infrastructure.Payloads.Responses;

internal sealed record GithubReleaseExternalResponse
{
    public string? TagName { get; set; }
    public List<GithubAssetExternalResponse>? Assets { get; set; }
}
