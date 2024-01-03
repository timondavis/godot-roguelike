using Godot;
using System;
using System.Threading;
using Roguelike.Map.Model;

public partial class RoomConnector : GodotObject
{
    public GeneratorGrid Grid { get; private set; }
    public TileType TileType { get; private set; }

    public RoomConnector(GeneratorGrid grid, TileType type)
    {
        Grid = grid;
        TileType = type;
    }
    
    public void ConnectRooms(Room roomA, Room roomB)
    {
       // Draw an imaginary line between the center of both squares 
       Vector2I start = roomA.Center;
       Vector2I end = roomB.Center;
       
       // Draw imaginary triangle to imagine city-block path
       Vector2I midPoint = new Vector2I(start.X, end.Y);
       
       // Draw the actual line
       Line(start, midPoint);
       Line(midPoint, end);
    }

    public void Line(Vector2I start, Vector2I end, bool replace = false)
    {
        Grid.MoveTo(start);

        Vector2I nextPosition = new Vector2I(start.X, start.Y);
        while (Grid.Current.Position != end)
        {
            if (!Grid.Current.IsActive)
            {
                Grid.Current.Activate(TileType);
            }

            int xMultiplier = (start.X).CompareTo(end.X) * -1;
            int yMultiplier = (start.Y).CompareTo(end.Y) * -1;

            nextPosition.X += (1 * xMultiplier);
            nextPosition.Y += (1 * yMultiplier);
            
            Grid.MoveTo(nextPosition);
        }
    }
}
