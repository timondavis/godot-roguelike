using System.Collections.Generic;

namespace Roguelike.Script.Map.Model.Direction.Pattern.RectangleTile;

public class SquarePatternSurround : DirectionalPattern
{
    public SquarePatternSurround()
    {
        Pattern = new HashSet<GridDirection>
        {
            GridDirection.North,
            GridDirection.NorthEast,
            GridDirection.East,
            GridDirection.SouthEast,
            GridDirection.South,
            GridDirection.SouthWest,
            GridDirection.West,
            GridDirection.NorthWest,
        };
    }
}