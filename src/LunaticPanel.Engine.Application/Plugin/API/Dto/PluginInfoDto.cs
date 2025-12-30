using LunaticPanel.Engine.Domain.Plugin.Enums;

namespace LunaticPanel.Engine.Application.Plugin.API.Dto;

public record PluginInfoDto(string Id, Version Version, PluginState State, PluginStartupState Bootup);
