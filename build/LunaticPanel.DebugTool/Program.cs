using LunaticPanel.DebugTool.Engine;
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