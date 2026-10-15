using LunaticPanel.Core.Utils.Abstraction.SafeFileWriter;
using LunaticPanel.Engine.Domain.Plugin.Enums;
using LunaticPanel.Engine.Web.Boostrap;
using System.Text.Json;

namespace LunaticPanel.Engine.Web.Services;

internal class BootstrapFileController : IBootstrapFileController
{
    private readonly ISafeFileWriter _safeFileWriter;
    private readonly string _bootstrapLocation = Path.Combine(Bootstrap.ConfigDirectory, "bootstrap.json");

    public BootstrapFileController(ISafeFileWriter safeFileWriter)
    {
        _safeFileWriter = safeFileWriter;
    }
    public async Task DisablePlugin(string id, CancellationToken ct = default)
        => await UpdateStateAsync(id, PluginStartupState.Disabled, ct);
    public async Task EnablePlugin(string id, CancellationToken ct = default)
        => await UpdateStateAsync(id, PluginStartupState.Enabled, ct);
    public async Task<BootstrapConfiguration> LoadFile(CancellationToken ct)
    {
        FileStream? fs = await WaitForLockAsync(_bootstrapLocation, ct);
        using StreamReader reader = new StreamReader(fs);
        var json = reader.ReadToEnd();
        reader.Close();
        fs.Close();
        return JsonSerializer.Deserialize<BootstrapConfiguration>(json)!;
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
    private async Task UpdateStateAsync(string id, PluginStartupState startupState, CancellationToken ct = default)
    {
        await _safeFileWriter.WriteThenCopyFileAsync(_bootstrapLocation, p =>
        {
            var bootstrap = JsonSerializer.Deserialize<BootstrapConfiguration>(p)!;
            var knownPlugins = bootstrap.KnownPlugins.ToList();
            var entity = knownPlugins.FirstOrDefault(p => p.Entity.Identity.PackageId == id);
            if (entity == default) return p;
            knownPlugins.Remove(entity);
            knownPlugins.Add(entity with
            {
                Entity = entity.Entity with
                {
                    Lifecycle = entity.Entity.Lifecycle with
                    {
                        StartupState = startupState
                    }
                }
            });
            bootstrap = bootstrap with
            {
                KnownPlugins = knownPlugins
            };
            return JsonSerializer.Serialize(bootstrap)!;

        });
    }
}
