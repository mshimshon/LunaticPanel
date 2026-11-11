using LunaticPanel.Core.Abstraction.Tools;
using System;

namespace LunaticPanel.PackageManager.Tests.Application.Mock;

public class PanelControl : IPanelControl
{
    public string PanelVersion { get; set; } = "1.0.0";

    public Guid Id { get; private set; } = Guid.NewGuid();

    public Task DashboardRender() => Task.CompletedTask;
    public Task LayoutRender() => Task.CompletedTask;
    public Task MenuRender() => Task.CompletedTask;
    public Task Shutdown() => Task.CompletedTask;
}
