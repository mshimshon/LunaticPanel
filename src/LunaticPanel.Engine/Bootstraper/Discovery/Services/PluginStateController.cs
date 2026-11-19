using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Web.Services;
using System;
using System.Text.Json;

namespace LunaticPanel.Engine.Web.Bootstraper.Discovery.Services;

public class PluginStateController : IPluginStateController
{
    private readonly string _fileLocation;

    public PluginStateController()
    {
        _fileLocation = Pathf.CombineEnsureCreated(Pathf.DirectorySeparatorChar, "var", "lib", "lunaticpanel", IBootstrapFileController.BootstrapFilename);

        EnsureCreated();

    }
    internal void EnsureCreated()
    {
        if (!File.Exists(_fileLocation))
            File.WriteAllText(_fileLocation, JsonSerializer.Serialize(new List<BootstrapStatusEntity>()));
    }
    public async Task DisablePlugin(string id, CancellationToken ct = default)
    {
        var info = await LoadFile(ct);
        var target = info.SingleOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (target == default) return;
        info.Remove(target);
        info.Add(target with { StartupState = BootstrapStartupState.Disabled });
        await SaveAsync(info, ct);
    }
    public async Task EnablePlugin(string id, CancellationToken ct = default)
    {
        var info = await LoadFile(ct);
        var target = info.SingleOrDefault(p => p.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (target == default) return;
        info.Remove(target);
        info.Add(target with { StartupState = BootstrapStartupState.Enabled });
        await SaveAsync(info, ct);
    }
    public Task<List<BootstrapStatusEntity>> LoadFile(CancellationToken ct = default)
    {
        EnsureCreated();
        var json = File.ReadAllText(_fileLocation);
        var result = JsonSerializer.Deserialize<List<BootstrapStatusEntity>>(json)!;
        return Task.FromResult(result);
    }

    public Task SaveAsync(List<BootstrapStatusEntity> list, CancellationToken ct = default)
    {
        File.WriteAllText(_fileLocation, JsonSerializer.Serialize(list));
        return Task.CompletedTask;
    }
}
