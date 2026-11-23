using LunaticPanel.Core.Abstraction.Tools;

namespace LunaticPanel.Engine.Infrastructure.Implementations;

public class GlobalTickerService : IGlobalTicker
{
    private long _currentTicker = long.MinValue;
    public long GetNext() => Interlocked.Increment(ref _currentTicker);
}
