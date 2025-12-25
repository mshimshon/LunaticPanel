using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Sonar", "S2326")]
public interface IPlugin
{
    void Initialize();
    void RegisterServices(IServiceCollection services);
    void Disable();
    void Enable();
}
