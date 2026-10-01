namespace LuncaticPanel.Package.Server.Infrastructure.Payloads.Responses;

public sealed record GithubAssetExternalResponse
{
    public string? Name { get; set; }
    public string? BrowserDownloadUrl { get; set; }
}
