using LunaticPanel.PackageManager.Application.Payloads;
using LunaticPanel.PackageManager.Domain.Respositories;

namespace LunaticPanel.PackageManager.Application.Services;

public interface IHostDiskPackageService : IPackageRepository
{
    Task<ICollection<PackagePayload>> GetPendingUpdates(CancellationToken ct = default);
    Task<ICollection<PackagePayload>> GetIntalled(CancellationToken ct = default);
    Task<ICollection<PackagePayload>> GetRollbacks(CancellationToken ct = default);
    Task<ICollection<PackagePayload>> GetPendingDelete(CancellationToken ct = default);
    Task<ICollection<PackagePayload>> GetPreInstalled(CancellationToken ct = default);
}
