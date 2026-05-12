namespace LunaticPanel.Core.Utils.Abstraction.Plugin.Location.Exceptions;

public class GlobalUserRequiredException : Exception
{
    public GlobalUserRequiredException() : base("Username is not set as global for the current context, you must set or use explicit methods.")
    {
    }
}