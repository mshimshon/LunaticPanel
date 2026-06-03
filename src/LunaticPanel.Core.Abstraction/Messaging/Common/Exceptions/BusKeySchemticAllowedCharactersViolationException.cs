namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusKeySchemticAllowedCharactersViolationException : BusException
{
    public BusKeySchemticAllowedCharactersViolationException(string id) :
        base(nameof(BusKeySchemticAllowedCharactersViolationException), $"{id} does not have a valid characters: 'a-Z 0-9 and .'")
    {

    }
}
