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

    private Vector2I SafeTarget(Vector2I target)
    {
        return new Vector2I(
            Math.Clamp(target.X, 0, Size.X),
            Math.Clamp(target.Y, 0, Size.Y)
        );
    }
    
    
}