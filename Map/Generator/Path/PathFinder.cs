using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Map.Model;

public partial class PathFinder : GodotObject
{
    public GeneratorGrid Grid { get; private set; }
    public TileType TileType { get; private set; }

    public PathFinder(GeneratorGrid grid, TileType type)
    {
        Grid = grid;
        TileType = type;
    }
    
    public Queue<RectangleRoom> FindRectangleRoomPath(HashSet<RectangleRoom> rooms)
    {
        // Sort rooms according to the position of their top left cell relative to (0,0)
        var orderedRooms = rooms.OrderBy(room => room.TopLeft.X)
            .ThenBy(room => room.TopLeft.Y )
            .ToList();

        return new Queue<RectangleRoom>(orderedRooms);

    }
}
