using System.Collections.Generic;

namespace Roguelike.Script.Map.Model.Direction.Pattern.RectangleTile;

public class SquarePatternStar : DirectionalPattern
{
    public SquarePatternStar()
    {
        Pattern = new HashSet<GridDirection>
        {
            GridDirection.Here,
            GridDirection.NorthWest,
            GridDirection.NorthEast,
            GridDirection.SouthWest,
            GridDirection.SouthEast,
        };
    }
}