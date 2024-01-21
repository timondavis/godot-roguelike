using Godot;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

public partial class RoomConnector : GodotObject
{
    public GeneratorGrid Grid { get; private set; }
    public TileType TileType { get; private set; }

    public RoomConnector(GeneratorGrid grid, TileType type)
    {
        Grid = grid;
        TileType = type;
    }
    
    public void ConnectRooms<TRoomShape>(Room<TRoomShape> roomA, Room<TRoomShape> roomB) where TRoomShape : Shape, new()
    {
       // Draw an imaginary line between the center of both squares 
       Vector2I start = roomA.Shape.Center;
       Vector2I end = roomB.Shape.Center;
       
       // Draw imaginary triangle to imagine city-block path
       Vector2I midPoint = new Vector2I(start.X, end.Y);
       
       // Draw the actual line
       Grid.MoveTo(start);
       Grid.LineTo(midPoint, TileType);
       Grid.LineTo(end, TileType);
    }
}
