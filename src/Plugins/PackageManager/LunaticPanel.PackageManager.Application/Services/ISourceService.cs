using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Services;

public interface ISourceService
{
    Task<ICollection<RepositorySourcePayload>> GetSourcesAsync(CancellationToken ct = default);
}
