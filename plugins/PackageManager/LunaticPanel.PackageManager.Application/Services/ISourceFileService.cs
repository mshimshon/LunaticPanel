using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Services;

public interface ISourceFileService
{
    Task<ICollection<RepositorySourcePayload>> GetSourcesAsync(CancellationToken ct = default);
    Task<ICollection<RepositorySourcePayload>> SaveSourcesAsync(List<RepositorySourcePayload> sourcePayloads, CancellationToken ct = default);
}
