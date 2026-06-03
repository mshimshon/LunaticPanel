namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusIdSchemticPrefixViolationException : Exception
{
    public BusIdSchemticPrefixViolationException(string id) : base($"{id} does not have a valid prefix 'EngineKey., QueryKey. or EventKey.'")
    {

    }
}
