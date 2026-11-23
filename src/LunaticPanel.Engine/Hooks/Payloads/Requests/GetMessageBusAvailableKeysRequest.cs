namespace LunaticPanel.Engine.Hooks.Payloads.Requests;

internal record GetMessageBusAvailableKeysRequest
{
    public bool IncludeEngine { get; set; } = true;
    public bool IncludeQuery { get; set; } = true;
    public bool IncludeEvent { get; set; } = true;
}
