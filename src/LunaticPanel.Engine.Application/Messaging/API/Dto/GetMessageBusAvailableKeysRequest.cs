namespace LunaticPanel.Engine.Application.Messaging.API.Dto;

internal record GetMessageBusAvailableKeysRequest
{
    public bool IncludeEngine { get; set; } = true;
    public bool IncludeQuery { get; set; } = true;
    public bool IncludeEvent { get; set; } = true;
}
