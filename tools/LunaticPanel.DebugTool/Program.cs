using LunaticPanel.DebugTool.Engine;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;
var tmp = Path.GetTempPath();
var cliTmp = Path.Combine(tmp, "lpcli");
var lockFile = Path.Combine(cliTmp, "cli.lock");
FileStream? lockStream = default;
try
{
    // The OS atomically checks/creates and locks the file handle to this process
    lockStream = new FileStream(
        lockFile,
        FileMode.OpenOrCreate,
        FileAccess.ReadWrite,
        FileShare.None // Blocks any other process or thread from touching it
    );
}
catch (Exception)
{
    Console.Error.WriteLine($"Antoher cli is running (lock: {lockFile})");
    Environment.Exit(1);
}
try
{

    var appBuilder = new ConsoleApplicationBuilder(args);
    var app = appBuilder.Build();
    await app.RunAsync();
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex.Message);
    Environment.Exit(1);
}
finally
{
    lockStream.Close();

}
