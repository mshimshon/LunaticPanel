using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Engine.Services;

public static class HostServiceStorage
{
    public static IReadOnlyCollection<ServiceDescriptor> HostServices { get; set; } = default!;
}
