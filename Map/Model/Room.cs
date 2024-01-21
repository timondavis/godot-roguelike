using Godot;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Model;

public partial class Room<TRoomShape> : GodotObject where TRoomShape : Shape, new()
{
    private static int _nextId = 1;
    public TRoomShape Shape;
    
    public int Id { get; private set; }

    public Room()
    {
        Shape = new TRoomShape();
        Id = _nextId;
        _nextId++;
    }
}
