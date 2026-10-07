namespace LunaticPanel.Core.Utils.Abstraction.LinuxCommand;

public interface ILinuxCommandResult
{
    IEnumerable<string> RawStandardOutput { get; }

    IEnumerable<string> RawStandardError { get; }
    string StandardOutput { get; }
    string StandardError { get; }
    string? Payload { get; }
    int ExitCode { get; }
    bool HasPayload { get; }
    bool Failed { get; }
    T DeserializeResult<T>(string? input);
}
