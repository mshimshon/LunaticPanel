namespace LunaticPanel.Core.Utils.Abstraction.IO;

public interface IAtomicFileSystem
{
    Task WriteAtomicAsync(string file, string content, CancellationToken ct = default, string? chown = default, string? chmod = default);
    Task WriteAtomicAsync(string file, IEnumerable<string> content, CancellationToken ct = default, string? chown = default, string? chmod = default);
    Task WriteAtomicAsync(string file, byte[] content, CancellationToken ct = default, string? chown = default, string? chmod = default);
    Task WriteAtomicAsync(string file, Func<string, string> updateContent, CancellationToken ct = default, string? chown = default, string? chmod = default);
    Task WriteAtomicAsync(string file, Func<string, IEnumerable<string>> updateContent, CancellationToken ct = default, string? chown = default, string? chmod = default);
    Task WriteAtomicAsync(string file, Func<string, byte[]> updateContent, CancellationToken ct = default, string? chown = default, string? chmod = default);

    Task<string> ReadAllTextAsync(string path, CancellationToken ct = default);
    Task MoveAsync(string source, string destination, bool overwrite, CancellationToken ct = default);
    Task CopyAsync(string source, string destination, bool overwrite, CancellationToken ct = default);
}
