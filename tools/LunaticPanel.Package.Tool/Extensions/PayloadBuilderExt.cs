using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.Package.Tool.Extensions;

internal static class PayloadBuilderExt
{
    private static JsonSerializerOptions _jsonSerializer = new()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        PropertyNameCaseInsensitive = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
#if DEBUG
        WriteIndented = true
#endif
    };
    public static Task PrintAsync(this object payload, CancellationToken ct = default)
    {
        string json = "";
        if (payload.GetType() == typeof(string))
            json = payload.ToString()!;
        else
            json = JsonSerializer.Serialize(payload, _jsonSerializer);
        string payloadString = $"<<<PAYLOAD_BEGIN>>>\n{json}\n<<<PAYLOAD_END>>>";
        Console.Out.WriteLine(payloadString);
        return Task.CompletedTask;
    }


}
