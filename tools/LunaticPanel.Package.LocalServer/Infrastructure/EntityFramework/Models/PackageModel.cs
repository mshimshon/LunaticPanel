using LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models.Contracts;

namespace LunaticPanel.Package.LocalServer.Infrastructure.EntityFramework.Models;

public sealed record PackageModel : IModelTimestamps
{
    public string Id { get; set; } = default!;
    public string? EndOfLifeMessage { get; set; }
    public ICollection<PackageInfoModel> Versions { get; set; } = default!;
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
