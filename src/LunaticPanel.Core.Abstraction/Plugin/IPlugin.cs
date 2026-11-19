using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Abstraction.DependencyInjection;
using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using LunaticPanel.Core.Abstraction.Messaging.Common;
using Microsoft.Extensions.Configuration;

namespace LunaticPanel.Core.Abstraction.Plugin;

public interface IPlugin : IPluginInfo
{
    void Configure(IConfiguration configuration);
    void OnCircuitStart(CircuitIdentity circuit);
    void OnCircuitEnd(CircuitIdentity circuit);
    IPluginContextService GetContext(Guid circuitId);
    void AddHostRedirectedServices(params HostRedirectionService[] serviceTypes);
    IReadOnlyList<MessageKey> CheckDependencyGracefully(Func<string, bool> isBusAvailable);

    IReadOnlyCollection<PluginValidationResult> PerformValidation();

    Task BeforeRuntimeStartAsync(IServiceProvider serviceProvider);

}
