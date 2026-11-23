using LunaticPanel.Core.Extensions;
using LunaticPanel.Core.Utils.Abstraction.IO;
using LunaticPanel.Engine.Bootstraper;
using LunaticPanel.Engine.Infrastructure.Interfaces;
using System.Text.Json;

namespace LunaticPanel.Engine.Infrastructure.Implementations;

internal class BootstrapFileController : IBootstrapFileController
{
    private readonly IAtomicFileSystem _atomicFileSystem;
    private readonly string _bootstrapLocation;


    public BootstrapFileController(IAtomicFileSystem AtomicFileSystem)
    {
        _bootstrapLocation = Pathf.CombineReadyFile([Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel"], IBootstrapFileController.BootstrapFilename);
        _atomicFileSystem = AtomicFileSystem;
    }
    public async Task DisablePlugin(string id, CancellationToken ct = default)
        => await UpdateStateAsync(id, BootstrapStartupState.Disabled, ct);
    public async Task EnablePlugin(string id, CancellationToken ct = default)
        => await UpdateStateAsync(id, BootstrapStartupState.Enabled, ct);
    public async Task<List<BootstrapStatusEntity>> LoadFile(CancellationToken ct)
    {
        FileStream? fs = await WaitForLockAsync(_bootstrapLocation, ct);
        using StreamReader reader = new StreamReader(fs);
        var json = await _atomicFileSystem.ReadAllTextAsync(_bootstrapLocation, ct);
        reader.Close();
        fs.Close();
        return JsonSerializer.Deserialize<List<BootstrapStatusEntity>>(json)!;
    }

    private async Task<FileStream> WaitForLockAsync(string file, CancellationToken ct = default)
    {
        int cycleTimeout = 0;
        while (true)
        {
            if (cycleTimeout >= 200)
            {
                // TODO: Throw
            }
            cycleTimeout++;

            try
            {
                return File.Open($"{file}.lock", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // lock is held by someone else
                await Task.Delay(50, ct);
            }
        }
    }
    private async Task UpdateStateAsync(string id, BootstrapStartupState startupState, CancellationToken ct = default)
    {
        await _atomicFileSystem.WriteAtomicAsync(_bootstrapLocation, p =>
        {
            var knownPlugins = JsonSerializer.Deserialize<List<BootstrapStatusEntity>>(p)!;
            var entity = knownPlugins.FirstOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (entity == default) return p;
            knownPlugins.Remove(entity);
            knownPlugins.Add(entity with
            {
                StartupState = startupState
            });
            return JsonSerializer.Serialize(knownPlugins)!;
        });
    }
}
