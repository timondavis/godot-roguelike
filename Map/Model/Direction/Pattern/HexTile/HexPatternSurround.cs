using System.Collections.Generic;

namespace Roguelike.Map.Model.Direction.Pattern.HexTile;

public class HexPatternSurround : DirectionalPattern
{
    public HexPatternSurround()
    {
        Pattern = new HashSet<GridDirection>
        {
            GridDirection.NorthEast,
            GridDirection.East,
            GridDirection.SouthEast,
            GridDirection.SouthWest,
            GridDirection.West,
            GridDirection.NorthWest,
        }; 
    }
}