using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;

namespace LunaticPanel.Core.Utils.SafeFileWriter;

internal sealed class SafeFileWriter : ISafeFileWriter
{
    public SafeFileWriter(ILinuxCommand linuxCommand)
    {
        _linuxCommand = linuxCommand;
    }
    private static readonly SemaphoreSlim _fileLock = new SemaphoreSlim(1, 1);
    private readonly ILinuxCommand _linuxCommand;

    public Task WriteThenCopyFileAsync(string file, string content, CancellationToken ct = default, string? chown = default, string? chmod = default)
        => WriteFileAsync(file, (tmpFile) => File.WriteAllTextAsync(tmpFile, content, ct), ct, chown, chmod);
    public Task WriteThenCopyFileAsync(string file, IEnumerable<string> content, CancellationToken ct = default, string? chown = default, string? chmod = default)
        => WriteFileAsync(file, (tmpFile) => File.WriteAllLinesAsync(tmpFile, content, ct), ct, chown, chmod);
    public Task WriteThenCopyFileAsync(string file, byte[] content, CancellationToken ct = default, string? chown = default, string? chmod = default)
        => WriteFileAsync(file, (tmpFile) => File.WriteAllBytesAsync(tmpFile, content, ct), ct, chown, chmod);


    private async Task<string?> GetPermissionForFileAsync(string file, CancellationToken ct = default)
    {
        var currentPermission = await _linuxCommand.BuildCommand($"stat -c \"%a\" \"{file}\"")
.PatchInStdOutAsPayload()
.ExecPayloadOrDefaultAsync<string>(string.Empty, ct);
        return currentPermission;
    }

    private async Task<string?> GetOwnerGroupForFileAsync(string file, CancellationToken ct = default)
    {
        var currentOwnerGroup = await _linuxCommand.BuildCommand($"stat -c \"%U:%G\" \"{file}\"")
            .PatchInStdOutAsPayload()
            .ExecPayloadOrDefaultAsync<string>(string.Empty, ct);
        return currentOwnerGroup;
    }

    private async Task FixFilePermissionsAsync(string file, string? chown, string? chmod, CancellationToken ct = default)
    {
        if (chown == default || chmod == default) return;
        var currentOwnerGroup = await _linuxCommand
        .BuildCommand($"chown {chown} \"{file}\"")
        .AndCommand($"chmod {chmod} \"{file}\"")
        .ExecAsync(ct);
    }
    private async Task WriteFileAsync(string file, Func<string, Task> onWrite, CancellationToken ct = default, string? chown = default, string? chmod = default)
    {

        // Wait for the gate to open
        await _fileLock.WaitAsync(ct);
        try
        {
            if (File.Exists(file))
            {
                if (chmod == default)
                    chmod = await GetPermissionForFileAsync(file, ct);
                if (chown == default)
                    chown = await GetOwnerGroupForFileAsync(file, ct);
            }
            string tmpFile = Path.GetTempFileName();
            await onWrite(tmpFile);
            File.Copy(tmpFile, file, true);
            await FixFilePermissionsAsync(file, chown, chmod, ct);
        }
        catch
        {
            throw;
        }
        finally
        {
            // Always release in a finally block so the next person in queue can enter
            _fileLock.Release();
        }
    }


}
