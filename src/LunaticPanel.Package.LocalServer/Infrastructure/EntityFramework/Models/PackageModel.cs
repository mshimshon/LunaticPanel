namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models;

public sealed record PackageModel
{
    public string Id { get; init; } = default!;
    public DateTime Created { get; init; }
    public DateTime Updated { get; init; }
    public string? EndOfLifeMessage { get; init; }
    public ICollection<PackageInfoModel> Versions { get; init; } = default!;
}
