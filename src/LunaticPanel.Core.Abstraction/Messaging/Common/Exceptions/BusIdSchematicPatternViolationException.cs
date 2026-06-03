namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusIdSchematicPatternViolationException : Exception
{
    public BusIdSchematicPatternViolationException(string id) :
        base($"{id} does not have a valid pattern 'Prefix.PluginId.VERSION.Discriminator.Internal.Id'")
    {

    }
}
