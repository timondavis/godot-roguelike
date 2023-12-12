using Godot;

namespace Roguelike.Map.Model;

public partial class GridCell : GodotObject
{
    public bool IsActive { get; set; }
    public Vector2I Position { get; set; }
}