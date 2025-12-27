namespace LunaticPanel.Engine.Domain.Plugin.Exceptions;

public class PluginFailedMustContainErrorMessageException : Exception
{
    public PluginFailedMustContainErrorMessageException() : base("A failure to load plugin must contain an error message.")
    {
    }
}
