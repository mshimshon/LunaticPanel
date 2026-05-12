namespace LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;

public interface ISafeFileWriter
{
    Task WriteThenCopyFileAsync(string file, string content, CancellationToken ct = default, string? chown = default, string? chmod = default);
    Task WriteThenCopyFileAsync(string file, IEnumerable<string> content, CancellationToken ct = default, string? chown = default, string? chmod = default);
    Task WriteThenCopyFileAsync(string file, byte[] content, CancellationToken ct = default, string? chown = default, string? chmod = default);
}
