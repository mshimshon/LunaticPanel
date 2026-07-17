using LunaticPanel.Package.LocalServer.Infrastructure;
using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;
using LuncaticPanel.Package.Server;
using LuncaticPanel.Package.Server.Web.Payloads.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddLunaPackage();
builder.Services.AddLocalServerInfrastructure(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}
app.UseLunaPackage();
app.UseLocalServerInfrastructure();
app.EnableLunaPackageCodedError();
app.UseLunaPackageCodedErrorFor<InfrastructureException>(p => new(p.Code, p.Message, ExceptionProvenencePayload.Infrastructure));
app.UseHttpsRedirection();
app.Run();