using System.Collections.Generic;

namespace Roguelike.Script.Map.Model.Direction.Pattern.HexTile;

public class HexPatternStar : DirectionalPattern
{
    public HexPatternStar()
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