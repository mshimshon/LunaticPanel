using LunaticPanel.Engine.Domain.Plugin.Enums;

namespace LunaticPanel.Engine.Application.Plugin.API.Dto;

public record PluginInfoResponse(string Id, Version Version, PluginState State, PluginStartupState Bootup);
