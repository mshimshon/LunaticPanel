namespace LunaticPanel.Core.Abstraction.Messaging.Common.Exceptions;

public class BusKeySchemticPrefixViolationException : BusException
{
    public BusKeySchemticPrefixViolationException(string id) :
        base(nameof(BusKeySchemticPrefixViolationException), $"{id} does not have a valid prefix 'EngineKey., QueryKey. or EventKey.'")
    {

    }
}
