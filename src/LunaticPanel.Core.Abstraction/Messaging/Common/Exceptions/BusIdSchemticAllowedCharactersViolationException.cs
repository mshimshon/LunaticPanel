namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusIdSchemticAllowedCharactersViolationException : Exception
{
    public BusIdSchemticAllowedCharactersViolationException(string id) : base($"{id} does not have a valid characters: 'a-Z 0-9 and .'")
    {

    }
}
