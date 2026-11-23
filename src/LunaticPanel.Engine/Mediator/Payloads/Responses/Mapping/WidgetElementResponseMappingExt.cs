using LunaticPanel.Engine.Models.Dashboard;
using LunaticPanel.Engine.Models.Dashboard.Enums;
using LunaticPanel.Engine.Models.Menu;
using System;
using System.Collections.Generic;
using System.Text;

namespace LunaticPanel.Engine.Mediator.Payloads.Responses.Mapping;

internal static class WidgetElementResponseMappingExt
{

    internal static WidgetElementModel ToModel(this WidgetElementResponse data)
    {
        return new() { Size = (WidgetSize)data.Size, Position = data.Position };
    }
}
