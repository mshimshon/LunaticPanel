using LunaticPanel.Package.Server.Application.Mediator.Engine;
using LunaticPanel.Package.Server.Application.Payloads.Requests;

namespace LunaticPanel.Package.Server.Application.Mediator.Commands;

public sealed record PackageUploadCommand(PackageUploadRequest Data) : IRequest
{
}
