using LunaticPanel.PackageManager.Application.Payloads;

namespace LunaticPanel.PackageManager.Application.Services;

public interface ISourceOrderingService
{
    Task MoveFirstAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default);
    Task MoveLastAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default);
    Task MoveUpAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default);
    Task MoveDownAsync(RepositorySourcePayload sourcePayload, CancellationToken ct = default);
}
