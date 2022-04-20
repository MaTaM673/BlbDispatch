using System.ComponentModel;
using System.Runtime.CompilerServices;
using Dispatch.WPF.Annotations;

namespace Dispatch.WPF.Models;
public class Unit : INotifyPropertyChanged
{
    public string CallSign { get; set; }
    public string Name { get; set; }
    public string FullName => $"[{CallSign}] {Name}";

    private State _currentState = new();
    public State CurrentState
    {
        get => _currentState;
        set
        {
            if (Equals(value, _currentState)) return;
            _currentState = value;
            OnPropertyChanged();
        }
    }

    private Postal? _currentPosition;
    public Postal? CurrentPosition
    {
        get => _currentPosition;
        set
        {
            if (Equals(value, _currentPosition)) return;
            _currentPosition = value;
            OnPropertyChanged();
        }
    }


    private Postal? _destination;
    public Postal? Destination
    {
        get => _destination;
        set
        {
            if (Equals(value, _destination)) return;
            _destination = value;
            OnPropertyChanged();
        }
    }

    public Unit(string callSign, string name)
    {
        CallSign = callSign;
        Name = name;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}