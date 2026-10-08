namespace LunaticPanel.Package.Server.Application.Payloads.Requests;

public record PackageUploadRequest
{
    public Guid UploadId { get; set; } = default!;
}
