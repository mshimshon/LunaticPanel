using LunaticPanel.Package.LocalServer.Web;
using LunaticPanel.Package.Server;

var builder = WebApplication.CreateBuilder(args);
builder.AddLunaPackage();

// Add services to the container.
builder.Services.AddLocalServerServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
var app = builder.Build();
Console.WriteLine($"Development: {app.Environment.IsDevelopment()}");
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });

}
else
    app.UseHttpsRedirection();
await app.UseLocalServerAPIEndpoints();
app.Run();