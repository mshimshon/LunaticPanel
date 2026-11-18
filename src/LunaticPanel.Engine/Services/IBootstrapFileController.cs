using LunaticPanel.Engine.Web.Boostrap;
using System.Reflection.Metadata;

namespace LunaticPanel.Engine.Web.Services;

internal interface IBootstrapFileController
{
    public static string bootstrapFile = "bootstrap.v1.json";
    Task DisablePlugin(string id, CancellationToken ct = default);
    Task EnablePlugin(string id, CancellationToken ct = default);
    Task<BootstrapConfiguration> LoadFile(CancellationToken ct = default);
}
