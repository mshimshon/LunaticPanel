using Microsoft.Extensions.DependencyInjection;

namespace LunaticPanel.Core.Abstraction.DependencyInjection;

public record HostRedirectionService(Type ServiceType, ServiceLifetime Lifetime);