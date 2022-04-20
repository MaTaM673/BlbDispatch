using Dispatch.WPF.Helpers;

namespace Dispatch.WPF.Models;
public class Unit : ObservableObject
{
    public string CallSign { get; set; }
    public string Name { get; set; }
    public string FullName => $"[{CallSign}] {Name}";

    private State _currentState = new();
    public State CurrentState
    {
        get => _currentState;
        set => RaiseAndSetIfChanged(ref _currentState, value);
    }

    private Postal? _currentPosition;
    public Postal? CurrentPosition
    {
        get => _currentPosition;
        set => RaiseAndSetIfChanged(ref _currentPosition, value);
    }


    private Postal? _destination;
    public Postal? Destination
    {
        get => _destination;
        set => RaiseAndSetIfChanged(ref _destination, value);
    }

    public Unit(string callSign, string name)
    {
        CallSign = callSign;
        Name = name;
    }
}