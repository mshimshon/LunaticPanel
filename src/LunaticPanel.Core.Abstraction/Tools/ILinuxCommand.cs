namespace LunaticPanel.Core.Abstraction.Tools;

public interface ILinuxCommand
{
    Task<TResponse> RunLinuxScriptWithReplyAs<TResponse>(string file, bool sudo = true);
    Task<string> RunLinuxScript(string file, bool sudo = true);
    Task<string> RunLinuxCommand(string command, bool sudo = true);
}
