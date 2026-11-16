using LunaticPanel.Engine.Domain.Plugin.Enums;
using NuGet.Versioning;

namespace LunaticPanel.Engine.Application.Plugin.API.Dto;

public record PluginInfoResponse(string Id, NuGetVersion Version, PluginState State, PluginStartupState Bootup);
