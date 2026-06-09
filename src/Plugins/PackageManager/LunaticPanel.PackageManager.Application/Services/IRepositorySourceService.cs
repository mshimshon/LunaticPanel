using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Domain.Entites.ValueObjects;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IRepositorySourceService
{
    ICollection<PackageInfoPayload> SearchAsync(string q, IReadOnlyCollection<RepositorySourceInfo> searchIn, CancellationToken ct = default);

}
