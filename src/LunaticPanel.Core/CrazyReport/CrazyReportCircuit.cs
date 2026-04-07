using LunaticPanel.Core.Abstraction.Circuit;
using LunaticPanel.Core.Utils.Logging;

namespace LunaticPanel.Core.CrazyReport;

internal class CrazyReportCircuit : ICrazyReportCircuit
{
    private readonly ICircuitRegistry _circuitRegistry;

    public CrazyReportCircuit(ICircuitRegistry circuitRegistry)
    {
        _circuitRegistry = circuitRegistry;
    }
    public Guid CircuitId => _circuitRegistry?.CurrentCircuit?.CircuitId ?? Guid.Empty;
}
