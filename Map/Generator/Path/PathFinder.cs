using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Grid;
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

    /// <summary>
    /// Finds the room path based on the position of their center cell relative to (0,0).
    /// </summary>
    /// <param name="rooms">The set of rooms to find path for.</param>
    /// <returns>A queue of rooms sorted in the path order.</returns>
    public Queue<Room> FindRoomPath(HashSet<Room> rooms)
    {
        // Sort rooms according to the position of their top left cell relative to (0,0)
        var orderedRooms = rooms.OrderBy(room =>
            {
                return room.Location.X;
            })
            .ThenBy(room =>
            {
                return room.Location.Y;
            })
            .ToList();

        return new Queue<Room>(orderedRooms);
    }
}
