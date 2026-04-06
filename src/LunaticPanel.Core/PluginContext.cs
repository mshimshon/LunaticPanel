using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.Plugin;
using LunaticPanel.Core.Abstraction.Widgets;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core;

public class PluginContext : IWidgetContext, IPluginContextService
{
    private readonly IServiceProvider _pluginServices;
    private readonly CircuitIdentity _circuit;
    private bool _isBlazorAttached;

    public Guid CircuitId => _circuit.CircuitId;

    public bool IsMasterCircuit => _circuit.IsMaster;
    public bool IsBlazorAttached { get => !IsMasterCircuit && _isBlazorAttached; set => _isBlazorAttached = value; }
    public PluginContext(IServiceProvider pluginServices, CircuitIdentity circuit)
    {
        _pluginServices = pluginServices;
        _circuit = circuit;
    }


    public TWidgetViewModel GetViewModel<TWidgetViewModel>() where TWidgetViewModel : IWidgetViewModel
        => _pluginServices.GetRequiredService<TWidgetViewModel>();
    public TService GetRequired<TService>() where TService : notnull => _pluginServices.GetRequiredService<TService>();


}
