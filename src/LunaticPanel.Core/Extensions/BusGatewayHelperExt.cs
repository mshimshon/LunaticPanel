using LunaticPanel.Core.Abstraction.Messaging.Common;
using LunaticPanel.Core.Abstraction.Plugin;

namespace LunaticPanel.Core.Extensions;

internal static class BusGatewayHelperExt
{
    private static bool CheckIfInternalId(string prefix, string id, string pluginId)
        => id.StartsWith($"{prefix}.[{pluginId}]", StringComparison.OrdinalIgnoreCase);


    public static bool isTargetInternalId(this IBusMessage busMessage, string pluginId)
    => CheckIfInternalId("enginekey", busMessage.GetKey(), pluginId) ||
       CheckIfInternalId("eventkey", busMessage.GetKey(), pluginId) ||
       CheckIfInternalId("querykey", busMessage.GetKey(), pluginId);

    public static bool IsTargetInternalKeyRegistered(this IPluginInfo pluginInfo, string id)
        => pluginInfo.Keys.Contains(id.ToLower());
}
