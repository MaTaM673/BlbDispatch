using System.Drawing;

namespace Dispatch.WPF.Models;
public class Postal
{
    public int Id { get; set; }
    public Point Position { get; set; }
    public int MapPositionX => Position.X - 75;
    public int MapPositionY => Position.Y - 75;

    public Postal(int id, Point position)
    {
        Id = id;
        Position = position;
    }
}
