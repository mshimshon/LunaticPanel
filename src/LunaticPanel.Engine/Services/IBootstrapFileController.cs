using LunaticPanel.Engine.Web.Boostrap;

namespace LunaticPanel.Engine.Web.Services;

internal interface IBootstrapFileController
{
    Task DisablePlugin(string id, CancellationToken ct = default);
    Task EnablePlugin(string id, CancellationToken ct = default);
    Task<BootstrapConfiguration> LoadFile(CancellationToken ct);
}
