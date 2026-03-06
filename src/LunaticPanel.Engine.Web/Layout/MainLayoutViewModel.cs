using LunaticPanel.Engine.Web.Services.PanelControl;
using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Layout;

public class MainLayoutViewModel : IDisposable
{
    private bool _loading = false;
    private bool _disposedValue;

    public bool Loading
    {
        get => _loading;
        private set
        {
            //if (value != _loading)
            //    _ = OnUpdate();
            _loading = value;
        }
    }
    private readonly IStatePulse _statePulse;
    private readonly PanelControl _panelControl;

    public Func<Task>? SpreadChanges { get; set; }

    public MainLayoutViewModel(IStatePulse statePulse, PanelControl panelControl)
    {
        _statePulse = statePulse;
        _panelControl = panelControl;
        _panelControl.LayoutStateHasChanged = SpreadChanges;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _panelControl.LayoutStateHasChanged = default;
            }

            _disposedValue = true;
        }
    }


    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
