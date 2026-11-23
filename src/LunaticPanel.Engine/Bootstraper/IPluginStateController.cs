using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Infrastructure.Interfaces;
using LunaticPanel.Engine.Plugin;
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
