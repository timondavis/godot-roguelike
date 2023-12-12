using System;
using System.Reflection;
using Godot;

namespace Roguelike.Map.Model;

public partial class GeneratorGrid : GodotObject
{
    // The width and height of the grid
    public Vector2I Size { get; private set; }
    
    // The mesh of cells on the map
    public GridCell[,] GridCells { get; private set; }
    
    // We have a pointer to index the position for traversal.
    public GridCell Current { get; private set; }

    public enum Direction
    {
        North = 0,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    };

    public GeneratorGrid(Vector2I size)
    {
        GridCells = new GridCell[size.X, size.Y];
        InitializeGrid();
    }
    
    /// <summary>
    /// Adjusts the position based on the given direction.
    /// </summary>
    /// <param name="position">The initial position</param>
    /// <param name="direction">The direction to move</param>
    /// <returns>The adjusted position</returns>
    /// <exception cref="ArgumentException">Thrown when an invalid direction is provided</exception>
    public static Vector2I AdjustPositionByDirection(Vector2I position, Direction direction)
    {
        Vector2I newPosition = new Vector2I(position.X, position.Y);

        switch (direction)
        {
            case Direction.North:
                newPosition.Y += 1;
                break;
            case Direction.NorthEast:
                newPosition.Y += 1;
                newPosition.X += 1;
                break;
            case Direction.East:
                newPosition.X += 1;
                break;
            case Direction.SouthEast:
                newPosition.Y -= 1;
                newPosition.X += 1;
                break;
            case Direction.South:
                newPosition.Y -= 1;
                break;
            case Direction.SouthWest:
                newPosition.Y -= 1;
                newPosition.X -= 1;
                break;
            case Direction.West:
                newPosition.X -= 1;
                break;
            case Direction.NorthWest:
                newPosition.Y += 1;
                newPosition.X -= 1;
                break;
            default:
                throw new ArgumentException(nameof(direction));
        }
        
        return newPosition;
    }

    public void MoveTo(Vector2I target)
    {
    }

    private void InitializeGrid()
    {
        for (int x = 0; x < Size.X; x++)
        {
            for( int y = 0 ; y < Size.Y; y++ )
            {
                GridCells[x, y] = new GridCell();
            }
        }
    }

    /// <summary>
    /// Returns a safe target point for a given target by clamping its coordinates to be within the boundaries of the Size.
    /// </summary>
    /// <param name="target">The target point to make safe.</param>
    /// <returns>A Vector2I representing the safe target point.</returns>
    protected Vector2I SafeTarget(Vector2I target)
    {
        return new Vector2I(
            Math.Clamp(target.X, 0, Size.X),
            Math.Clamp(target.Y, 0, Size.Y)
        );
    }

    /// <summary>
    /// Adjusts position by a given direction and returns the safe target
    /// </summary>
    /// <param name="position">The original position</param>
    /// <param name="direction">The direction of adjustment</param>
    /// <returns>The adjusted position as a Vector2I</returns>
    protected Vector2I SafeAdjustPositionByDirection(Vector2I position, Direction direction)
    {
        Vector2I adjusted = AdjustPositionByDirection(position, direction);
        return SafeTarget(adjusted);
    }
    
    
    
    
}