namespace LunaticPanel.DebugTool.Extensions;

internal static class MSBuildExt
{
    public static async Task PublishProjectAsync(string project)
    {
        Console.Out.WriteLine($"Publishing '{project}'");
        string filename = Path.GetFileNameWithoutExtension(project);
        var tmp = Path.GetTempPath();
        var cliTmp = Path.Combine(tmp, "lpcli");
        var cliPublishTmp = Path.Combine(cliTmp, "publish");
        Console.Out.WriteLine($"Temp Publish At '{cliPublishTmp}'");
        if (!Directory.Exists(cliPublishTmp))
            Directory.CreateDirectory(cliPublishTmp);
        var cliProjectOutput = Path.Combine(cliPublishTmp, filename);
        Console.Out.WriteLine($"Temp Publish Target At '{cliProjectOutput}'");
        if (Directory.Exists(cliProjectOutput))
            Directory.Delete(cliProjectOutput, true);
        string cmd = $"publish \"{project}\" -c Debug -r linux-x64 --self-contained true -o \"{cliProjectOutput}\"";
        Console.Out.WriteLine($"dotnet {cmd}");

        await ProcessExt.RunProcessAsync("dotnet", cmd);
    }
}
