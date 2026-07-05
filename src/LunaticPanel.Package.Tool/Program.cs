using LunaticPanel.Package.Tool.Engine;
var b = WebApplication.CreateBuilder(args);
var builder = new ConsoleApplicationBuilder(args);
var app = builder.Build();


await app.RunAsync();