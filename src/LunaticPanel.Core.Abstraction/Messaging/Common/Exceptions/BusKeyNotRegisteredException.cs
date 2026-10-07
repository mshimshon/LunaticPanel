namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusKeyNotRegisteredException : BusException
{
    public BusKeyNotRegisteredException(string id, string pluginId) :
        base(nameof(BusKeyNotRegisteredException), $"{id} is not registered within {pluginId}.")
    {

    }
}
