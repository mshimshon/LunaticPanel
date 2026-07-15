using LunaticPanel.Package.LocalServer.Infrastructure.Exceptions;
using LuncaticPanel.Package.Server;
using LuncaticPanel.Package.Server.Web.Payloads.Enums;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddLunaPackage();

var app = builder.Build();
app.UseLunaPackage();
app.EnableLunaPackageCodedError();
app.UseLunaPackageCodedErrorFor<InfrastructureException>(p => new(p.Code, p.Message, ExceptionProvenencePayload.Infrastructure));
app.UseHttpsRedirection();
app.Run();