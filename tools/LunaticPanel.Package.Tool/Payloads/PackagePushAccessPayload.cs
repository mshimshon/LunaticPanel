namespace LunaticPanel.Package.Tool.Payloads;

public class PackagePushAccessPayload
{
    public string UploadId { get; set; } = default!;
    public string Provider { get; set; } = default!;
    public Dictionary<string, string> Headers { get; set; } = default!;
}
