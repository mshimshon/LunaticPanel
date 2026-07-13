namespace LuncaticPanel.Package.Server.Application.Payloads.Requests;

public sealed record EndOfLifeRequest
{
    public string Id { get; init; } = default!;
    public string Message { get; init; } = default!;

}
