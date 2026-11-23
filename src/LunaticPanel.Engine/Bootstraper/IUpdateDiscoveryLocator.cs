using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Extensions;
using LunaticPanel.Engine.Plugin;

using NuGet.Versioning;
using System;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace LunaticPanel.Engine.Bootstraper;

public interface IUpdateDiscoveryLocator
{

    string BinaryPathRoot { get; }
    string VarLibRoot { get; }
    string UpgradeRoot { get; }
    string UpgradeReadyApply { get; }
    string UpgradePluginInstalled { get; }
    string UpgradePluginPreinstalled { get; }
    string RuntimeBackupRoot { get; }
    string UpgradePluginCache { get; }
    void Initialize();

}
