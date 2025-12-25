using StatePulse.Net;
using SwizzleV;

namespace LunaticPanel.Engine.Layout.ViewModels;

public class MainLayoutViewModel
{
    private bool _loading = false;
    public bool Loading
    {
        get => _loading;
        private set
        {
            if (value != _loading)
                _ = OnUpdate();
            _loading = value;
        }
    }
    private readonly ISwizzleViewModel _swizzleViewModel;
    private readonly IStatePulse _statePulse;

    private async Task OnUpdate() => await _swizzleViewModel.SpreadChanges(() => this);
    public MainLayoutViewModel(ISwizzleViewModel swizzleViewModel, IStatePulse statePulse)
    {
        _swizzleViewModel = swizzleViewModel;
        _statePulse = statePulse;
    }


}
