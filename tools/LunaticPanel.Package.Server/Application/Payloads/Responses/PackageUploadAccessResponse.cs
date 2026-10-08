namespace LunaticPanel.Package.Server.Application.Payloads.Responses;

public sealed record PackageUploadAccessResponse(string UploadId, string Provider, Dictionary<string, string> Headers)
{
}
