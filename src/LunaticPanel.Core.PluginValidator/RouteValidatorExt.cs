using LunaticPanel.Core.Abstraction;
using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using Microsoft.AspNetCore.Components;
using System.Reflection;

namespace LunaticPanel.Core.PluginValidator;

public static class RouteValidatorExt
{
    public static PluginValidationResult FindAnyInvalidRoutesNames(this IPlugin plugin)
    {
        var invalidRoutes = new List<string>();
        List<PluginValidationError> validationErrors = new();
        foreach (var type in plugin.GetType().Assembly.GetTypes())
        {
            if (!typeof(ComponentBase).IsAssignableFrom(type))
                continue;

            var routes = type.GetCustomAttributes<RouteAttribute>(inherit: false);

            foreach (var route in routes)
            {
                var template = route.Template.TrimStart('/');

                if (!template.StartsWith(plugin.PluginId, StringComparison.OrdinalIgnoreCase))
                {
                    validationErrors.Add(new()
                    {
                        Message = $"{type.FullName} → /{template}",
                        Origin = type.FullName!
                    });
                    Console.WriteLine($"ERROR NAMING ROUTE: {type.FullName} → /{template} should be → /{type.FullName}/{template} ");
                    invalidRoutes.Add($"{type.FullName} → /{template}");
                }
            }
        }
        PluginValidationResult result = new()
        {
            Errors = validationErrors.AsReadOnly(),
            PluginId = plugin.PluginId
        };
        return result;
    }

}
