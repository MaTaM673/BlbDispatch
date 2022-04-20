using System.Windows.Media;

namespace Dispatch.WPF.Models;
public class State
{
    public string? Name { get; set; }
    public Color Color { get; set; }

    public State(){}

    public State(string name, Color color)
    {
        Name = name;
        Color = color;
    }
}
