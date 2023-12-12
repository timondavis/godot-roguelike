using System;
using System.Collections.Generic;
using System.Net;
using Godot;
using Roguelike.Map.Model;


namespace Roguelike.Map.Generator;

public partial class LifeMapGenerator : MapGenerator
{
    [Export(PropertyHint.Range, "0.01, 1.00, 0.01" )] 
    public decimal StartingDensity { get; set; } 
    
    public override void GenerateGrid()
    {
        InitializeGrid();
        var numberOfStartPoints = HowManyStartPoints();
        GenerateStartPoints(numberOfStartPoints);
        var a = 1;
    }

    private int HowManyStartPoints()
    {
        var value = (int) Math.Round(Width * Height * StartingDensity);
        return Math.Max(1, value);
    }

    private void GenerateStartPoints(int howManyPoints)
    {
        int x, y;

        HashSet<GeneratorGrid.Direction> starPattern = GetStarPattern();
        HashSet<GeneratorGrid.Direction> plusPattern = GetPlusPattern();
        
        // Just in case we can't find any more unique values, set a threshold and count failed attempts
        // To generate Unique values.
        GD.Randomize();
        for (int i = 0; i < howManyPoints ; i++) 
        {
            x = (int)GD.RandRange(0, Width);
            y = (int)GD.RandRange(0, Height);
            Grid.MoveTo(new Vector2I(x, y));

            Godot.Collections.Dictionary<GeneratorGrid.Direction, GridCell> cells;

            if (i % 2 == 0)
            {
                cells = Grid.RelativeQuery(starPattern);
            }
            else
            {
                cells = Grid.RelativeQuery(plusPattern);
            }
            
            foreach (var cell in cells)
            {
                if (cell.Value != null)
                {
                    cell.Value.IsActive = true;
                } 
            }
        }
    }

    private HashSet<GeneratorGrid.Direction> GetStarPattern()
    {
        return new HashSet<GeneratorGrid.Direction>
        {
            GeneratorGrid.Direction.Here,
            GeneratorGrid.Direction.NorthWest,
            GeneratorGrid.Direction.NorthEast,
            GeneratorGrid.Direction.SouthWest,
            GeneratorGrid.Direction.SouthEast,
        }; 
    }

    private HashSet<GeneratorGrid.Direction> GetPlusPattern()
    {
        return new HashSet<GeneratorGrid.Direction>
        {
            GeneratorGrid.Direction.Here,
            GeneratorGrid.Direction.East,
            GeneratorGrid.Direction.South,
            GeneratorGrid.Direction.West,
            GeneratorGrid.Direction.North
        }; 
    }
}