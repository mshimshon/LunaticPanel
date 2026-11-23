using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Bootstraper;
using System.Reflection.Metadata;

namespace LunaticPanel.Engine.Infrastructure.Interfaces;

internal interface IBootstrapFileController
{
    public static string BootstrapFilename { get; } = "bootstrap.v1.json";
    Task DisablePlugin(string id, CancellationToken ct = default);
    Task EnablePlugin(string id, CancellationToken ct = default);
    Task<List<BootstrapStatusEntity>> LoadFile(CancellationToken ct = default);
}
