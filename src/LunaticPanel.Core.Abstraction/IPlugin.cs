using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using Microsoft.Extensions.Configuration;

namespace LunaticPanel.Core.Abstraction;

public interface IPlugin
{
    void Configure(IConfiguration configuration);
    void OnCircuitStart(CircuitIdentity circuit);
    void OnCircuitEnd(CircuitIdentity circuit);
    IPluginContextService GetContext(Guid circuitId);
    void AddHostRedirectedServices(params HostRedirectionService[] serviceTypes);
    string PluginId { get; }

    IReadOnlyCollection<PluginValidationResult> PerformValidation();

}
