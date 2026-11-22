using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;
using LunaticPanel.Engine.Services;
using NuGet.Versioning;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace LunaticPanel.Engine.Bootstraper;

internal interface IPluginStateController : IBootstrapFileController
{

    Task SaveAsync(List<BootstrapStatusEntity> list, CancellationToken ct = default);
}
