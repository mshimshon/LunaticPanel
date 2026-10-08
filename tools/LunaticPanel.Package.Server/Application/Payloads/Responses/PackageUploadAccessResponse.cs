namespace LunaticPanel.Package.Server.Application.Payloads.Responses;

public sealed record PackageUploadAccessResponse(string Provider, Dictionary<string, string> Metadata)
{
}
