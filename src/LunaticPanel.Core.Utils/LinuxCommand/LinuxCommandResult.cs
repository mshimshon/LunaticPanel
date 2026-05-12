using LunaticPanel.Core.Utils.Abstraction.LinuxCommand;
using LunaticPanel.Core.Utils.Abstraction.LinuxCommand.Exceptions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace LunaticPanel.Core.Utils.LinuxCommand;

public sealed class LinuxCommandResult : ILinuxCommandResult
{

    public IEnumerable<string> RawStandardOutput { get; init; } = default!;
    public IEnumerable<string> RawStandardError { get; init; } = default!;
    public string StandardOutput { get; } = string.Empty;
    public string StandardError { get; init; } = string.Empty;
    public string? Payload { get; init; }
    public bool HasPayload => Payload != default;
    public int ExitCode { get; init; } = int.MinValue;
    public bool Failed => ExitCode != int.MinValue ? ExitCode != 0 : !string.IsNullOrWhiteSpace(StandardError);
    public LinuxCommandResult(string standardOutput)
    {
        StandardOutput = standardOutput;
        Payload = ExtractLastPayload(StandardOutput);

    }

    public LinuxCommandResult() { }


    public T DeserializeResult<T>(string? input)
    {
        try
        {
            if (Failed)
                throw new PayloadFailedToDeserializeException(input);
            if (input == default)
                throw new PayloadFailedToDeserializeException(input, "Payload is null");
            if (typeof(T) == typeof(string))
            {
                object resultDirectInput = input;
                return (T)resultDirectInput;
            }


            object? resultObj = default;

            if (typeof(T) == typeof(bool))
                resultObj = bool.Parse(input);
            else if (typeof(T) == typeof(TimeSpan))
                resultObj = TimeSpan.Parse(input);
            else if (typeof(T) == typeof(DateTime))
                resultObj = DateTime.Parse(input);
            else if (typeof(T) == typeof(Guid))
                resultObj = Guid.Parse(input);

            if (resultObj != default)
                return (T)resultObj;
            JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                AllowTrailingCommas = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                ReadCommentHandling = JsonCommentHandling.Skip
            };
            var expectResult = JsonSerializer.Deserialize<T>(input, jsonSerializerOptions);
            if (expectResult == null)
                throw new PayloadFailedToDeserializeException(input, "Command failed result is null.");
            return expectResult;
        }
        catch (Exception ex)
        {
            throw new PayloadFailedToDeserializeException(input, ex.Message);
        }
    }

    public static bool TryDecodeB64(string? input, out string? decoded)
    {
        decoded = default;
        if (string.IsNullOrWhiteSpace(input)) return false;
        bool failsDividibility = input.Length % 4 != 0;
        bool failsCharacterConstrains = !Regex.IsMatch(input, @"^[A-Za-z0-9+/=]+$");

        bool failsPreChecks = failsDividibility || failsCharacterConstrains;
        if (failsPreChecks) return false;
        int pad = input.Count(ch => ch == '=');
        bool failsPadding = pad > 2 || pad > 0 && !input.EndsWith(new string('=', pad));
        if (failsPadding) return false;

        try
        {
            byte[]? bytes = Convert.FromBase64String(input);
            decoded = Encoding.UTF8.GetString(bytes);
            return true;
        }
        catch
        {
            return false;
        }
    }
    private string? ExtractLastPayload(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return default;
        var matches = Regex.Matches(
            input,
            @"<<<PAYLOAD_BEGIN>>>([\s\S]*?)<<<PAYLOAD_END>>>",
            RegexOptions.Multiline
        );
        string? payloadUnProcessed = matches.Count > 0
            ? matches[^1].Groups[1].Value
            : null;
        if (TryDecodeB64(payloadUnProcessed, out string? decoded))
        {
            return decoded;
        }
        else if (payloadUnProcessed == default)
            return default;
        else
        {
            return payloadUnProcessed;
        }

    }


}
