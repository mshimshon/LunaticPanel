using StatePulse.Net;

namespace LunaticPanel.Engine.Web.Layout.ViewModels;

public class MainLayoutViewModel
{
    private bool _loading = false;
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

    public MainLayoutViewModel(IStatePulse statePulse)
    {
        _statePulse = statePulse;
    }


}
