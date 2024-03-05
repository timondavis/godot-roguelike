using System.Collections.Generic;
using Roguelike.Script.Map.Model.Direction.Pattern;
namespace Roguelike.Script.Map.Model.Direction.Pattern.HexTile;

public class HexPatternPlus : DirectionalPattern
{
    public HexPatternPlus()
    {
         Pattern = new HashSet<GridDirection>
         {
             GridDirection.Here,
             GridDirection.NorthWest,
             GridDirection.SouthEast,
             GridDirection.West,
             GridDirection.East
         }; 
    } 
}