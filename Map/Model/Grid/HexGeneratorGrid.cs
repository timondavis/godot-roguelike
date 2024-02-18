using System;
using Godot;
using Roguelike.Map.Model.Direction;

namespace Roguelike.Map.Model.Grid;

public partial class HexGeneratorGrid : GeneratorGrid
{
    public HexGeneratorGrid(Vector2I size) : base(size)
    {
        TileShape = TileSet.TileShapeEnum.Hexagon;
    }
    
    public override Vector2I AdjustPositionByDirection(Vector2I position, GridDirection direction)
    {
        Vector2I newPosition = new Vector2I(position.X, position.Y);

        switch (direction)
        {
            case GridDirection.NorthEast:
                newPosition.Y -= 1;
                newPosition.X += (position.Y % 2 == 0) ? 1 : 0;
                break;
            case GridDirection.East:
                newPosition.X += 1;
                break;
            case GridDirection.SouthEast:
                newPosition.Y += 1;
                newPosition.X += (position.Y % 2 == 0) ? 1 : 0;
                break;
            case GridDirection.SouthWest:
                newPosition.Y += 1;
                newPosition.X -= (position.Y % 2 == 0 ) ? 0 : 1;
                break;
            case GridDirection.West:
                newPosition.X -= 1;
                break;
            case GridDirection.NorthWest:
                newPosition.Y -= 1;
                newPosition.X -= (position.Y % 2 == 0 ) ? 0 : 1;
                break;
            case GridDirection.Here:
                break;
            default:
                throw new ArgumentException(nameof(direction));
        }
		
        return newPosition;
    }
}