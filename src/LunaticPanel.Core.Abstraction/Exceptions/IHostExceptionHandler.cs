namespace LunaticPanel.Core.Abstraction.Exceptions;

public interface IHostExceptionHandler
{
    void Throw(Exception ex);
    void Throw(HostCodedException ex);
    Task ThrowAsync(Exception ex, CancellationToken ct = default);
    Task ThrowAsync(HostCodedException ex, CancellationToken ct = default);
}
