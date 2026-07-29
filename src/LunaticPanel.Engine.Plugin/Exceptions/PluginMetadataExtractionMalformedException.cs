using LunaticPanel.Core.Abstraction.Exceptions;

namespace LunaticPanel.Engine.Plugin.Exceptions;

public sealed class PluginMetadataExtractionMalformedException : HostCodedException
{
    public PluginMetadataExtractionMalformedException(string prop, string message) :
        base(nameof(PluginMetadataExtractionMalformedException), message)
    {
        Prop = prop;
    }

    public string Prop { get; }
}
