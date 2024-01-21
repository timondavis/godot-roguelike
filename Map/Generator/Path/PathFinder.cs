using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

public partial class PathFinder : GodotObject
{
    public GeneratorGrid Grid { get; private set; }
    public TileType TileType { get; private set; }

    public PathFinder(GeneratorGrid grid, TileType type)
    {
        Grid = grid;
        TileType = type;
    }
    
    public Queue<Room<Rectangle>> FindRectangleRoomPath(HashSet<Room<Rectangle>> rooms)
    {
        // Sort rooms according to the position of their top left cell relative to (0,0)
        var orderedRooms = rooms.OrderBy(room => room.Shape.TopLeft.X)
            .ThenBy(room => room.Shape.TopLeft.Y )
            .ToList();

        return new Queue<Room<Rectangle>>(orderedRooms);

    }
}
