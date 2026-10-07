namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusKeySchematicPatternViolationException : BusException
{
    public BusKeySchematicPatternViolationException(string id) :
        base(nameof(BusKeySchematicPatternViolationException), $"{id} does not have a valid pattern 'Prefix.PluginId.VERSION.Discriminator.Internal.Id'")
    {

    }
}
