using LunaticPanel.Core.Abstraction;
using LunaticPanel.Core.Abstraction.Diagnostic.Messages;
using LunaticPanel.Core.Abstraction.Widgets;
using Microsoft.AspNetCore.Components;

namespace LunaticPanel.Core.PluginValidator;

public static class ComponentValidatorExt
{
    private static List<string> _exempted = [
        "_Imports"
        ];
    private static bool InheritsOpenGeneric(Type type, Type openGeneric)
    {
        while (type != null && type != typeof(object))
        {
            var current = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
            if (current == openGeneric)
                return true;

            type = type.BaseType!;
        }

        return false;
    }

    public static PluginValidationResult FindAnyWidgetNotUsingProperComponentBase(this IPlugin plugin)
    {
        List<PluginValidationError> validationErrors = new();
        foreach (var type in plugin.GetType().Assembly.GetTypes())
        {
            if (type == null)
                continue;

            if (!typeof(ComponentBase).IsAssignableFrom(type))
                continue;

            if (!type.IsClass || type.IsAbstract)
                continue;
            if (_exempted.Contains(type.Name))
                continue;
            // Check inheritance chain for WidgetComponentBase<,>
            var current = type;
            var openGeneric = typeof(WidgetComponentBase<,>);
            var openGeneric2 = typeof(WidgetComponentBase<>);
            var inherits = InheritsOpenGeneric(current, openGeneric) || InheritsOpenGeneric(current, openGeneric2);
            if (!inherits)
            {
                validationErrors.Add(new()
                {
                    Message = $"{type.FullName} does not inherit {nameof(WidgetComponentBase<,>)}... all plugin components must inherit it and use the view model pattern.",
                    Origin = type.FullName!
                });
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
