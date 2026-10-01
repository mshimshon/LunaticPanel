namespace LunaticPanel.PackageManager.Application.Payloads.Requests;

public sealed record SearchRequest
{
    private int _maxResult = 10;

    public string Keywords { get; set; } = default!;
    public int Position { get; set; }
    public int MaxResult
    {
        get => _maxResult; set
        {
            if (value <= 0)
                _maxResult = 10;
            else if (value > 50)
                _maxResult = 50;
            else _maxResult = value;
        }
    }
}
