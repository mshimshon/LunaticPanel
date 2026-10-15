using LunaticPanel.Package.Tool.Exceptions;
using LunaticPanel.Package.Tool.Extensions;
using LunaticPanel.Package.Tool.Payloads;
using LunaticPanel.Package.Tool.Tools.Validation;
using System.CommandLine;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LunaticPanel.Package.Tool.Tools.Push;

public static class PushCommandExt
{
    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
#if DEBUG
        WriteIndented = true,
#endif
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
    };
    public static RootCommand WithPushCommands(this RootCommand root)
    {
        var command = new Command("push", "pack plugin folder to .lpkg")
            .AddOption<string>("input", "in", "lpkg file.")
            .AddOption<string>("source", "src", "LunaPackage Endpoint API")
            .AddOption<string>("headers", "hd", "headers \"key:value;key:value\"")
            .SetExecuteCommand(PushingAction);
        return root.WithSubCommand(command);
    }
    private static async Task<PluginManifestPayload> PushingAction(ParseResult parseResult, CancellationToken ct = default)
    {
        var input = parseResult.GetValue<string>("--input");
        var source = parseResult.GetValue<string>("--source");
        bool missingParams = input == default || source == default;
        if (missingParams)
            throw new MissingParametersException("--input or --output is missing and required for packing.");
        else if (!File.Exists(input))
            throw new PushInputFileMisingException();
        Uri sourceUrl = new(source!);

        var headers = parseResult.GetValue<string>("--headers");
        Dictionary<string, string> requestHeaders = new();
        try
        {
            if (headers != default)
                requestHeaders = headers.Split(';').Select(p => p.Split(':')).ToDictionary(p => p[0], p => p[1]);
        }
        catch (Exception ex)
        {
            throw new PushHeadersIncorrectException();
        }

        PluginManifestPayload manifest = await PluginValidatorCommandExt.ValidatePackageAsync(input);
        PackagePushAccessPayload uploadPermissionAccess = await RequestUploadPermission(manifest, sourceUrl!, requestHeaders);
        await UploadFile(input, sourceUrl!, uploadPermissionAccess, uploadPermissionAccess.Headers);
        await CompleteUpload(uploadPermissionAccess.UploadId, sourceUrl!, requestHeaders);
        return manifest;
    }

    private static async Task<PackagePushAccessPayload> RequestUploadPermission(PluginManifestPayload manifest, Uri source, Dictionary<string, string> headers)
    {
        using HttpClient? client = new HttpClient();
        Console.WriteLine($"Requesting Access '{source}'");
        client.BaseAddress = source;
        var json = JsonSerializer.Serialize(manifest, _jsonSerializerOptions);
        var body = new StringContent(json, Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Get, "/lpkg/v1/package/push")
        {
            Content = body
        };
        // add or update headers
        foreach (var kv in headers)
        {
            // If header already exists, remove then add
            if (request.Headers.Contains(kv.Key))
                request.Headers.Remove(kv.Key);

            request.Headers.Add(kv.Key, kv.Value);
        }
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode && result != default)
            Console.WriteLine(result);
        response.EnsureSuccessStatusCode();
        Console.WriteLine($"Access Granted for '{source}'");



        var returnValue = JsonSerializer.Deserialize<PackagePushAccessPayload>(result, _jsonSerializerOptions);
        if (returnValue == default!)
            throw new PushAccessResponseInvalidException(result);
        Console.WriteLine($"Upload must be done at '{returnValue.Provider}'");

        return returnValue;

    }

    private static async Task UploadFile(string input, Uri source, PackagePushAccessPayload access, Dictionary<string, string> headers)
    {
        using var client = new HttpClient();
        Uri? endpoint = default;
        if (access.Provider.StartsWith("http"))
            endpoint = new Uri(access.Provider);
        else
            endpoint = new Uri(source, access.Provider);

        Console.WriteLine($"Upload file to '{endpoint}'");


        await using var fileStream = File.OpenRead(input);
        var content = new MultipartFormDataContent
        {
            { new StreamContent(fileStream), "file", Path.GetFileName(input) }
        };
        var response = await client.PostAsync(endpoint, content);
        var result = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode && result != default)
            Console.WriteLine(result);
        response.EnsureSuccessStatusCode();
        Console.WriteLine(result);
    }

    private static async Task CompleteUpload(string uploadId, Uri source, Dictionary<string, string> headers)
    {
        using var client = new HttpClient();
        client.BaseAddress = source;
        var request = new HttpRequestMessage(HttpMethod.Post, $"/lpkg/v1/package/push/{uploadId}");
        foreach (var kv in headers)
        {
            if (request.Headers.Contains(kv.Key))
                request.Headers.Remove(kv.Key);
            request.Headers.Add(kv.Key, kv.Value);
        }
        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();

    }
}
