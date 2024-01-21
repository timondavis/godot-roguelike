using Godot;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Model;

public partial class Room<TRoomShape> : GodotObject where TRoomShape : Shape
{
    private static int _nextId = 1;
    public TRoomShape Shape;
    
    public int Id { get; private set; }
    
    public Room(TRoomShape shape)
    {
        Shape = shape;
        InitializeCore();
    }

    private void InitializeCore()
    {
        Id = _nextId;
        _nextId++;
    }
}
