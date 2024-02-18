using System.Collections.Generic;

namespace Roguelike.Map.Model.Direction.Pattern.RectangleTile;

public class SquarePatternPlus : DirectionalPattern
{
    public SquarePatternPlus()
    {
         Pattern = new HashSet<GridDirection>
        {
            GridDirection.Here,
            GridDirection.East,
            GridDirection.South,
            GridDirection.West,
            GridDirection.North
        };  
    }
}