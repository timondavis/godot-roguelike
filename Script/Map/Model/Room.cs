using Godot;
using Roguelike.Script.Map.Model.Shapes;

namespace Roguelike.Script.Map.Model;

public abstract partial class Room
{
    private static int _nextId = 1;
    
    public int Id { get; private set; }
    public abstract Vector2I Location { get; }
    public abstract Vector2I Size { get; }
    
    public Room()
    {
        Id = _nextId;
        _nextId++;
    }
}
