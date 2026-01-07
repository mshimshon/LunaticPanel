using LunaticPanel.Core.Abstraction.Circuit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Abstraction;

public interface IPlugin
{
    void Configure(IConfiguration configuration);
    void OnCircuitStart(CircuitIdentity circuit);
    void OnCircuitEnd(CircuitIdentity circuit);
    IPluginContextService GetContext(Guid circuitId);
    void AddHostRedirectedServices(IServiceCollection serviceDescriptors);
    string PluginId { get; }

}
