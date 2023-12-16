using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using Godot;
using Roguelike.Map.Model;


namespace Roguelike.Map.Generator;

public partial class LifeMapGenerator : MapGenerator
{
	[Export(PropertyHint.Range, "0.01, 1.00, 0.01" )] 
	public float StartingDensity { get; set; }
	
	[Export( PropertyHint.Range, "1, 1000, 1)" )]
	public int LifeCycles { get; set; }
	
	[Export]
	public float CycleEmissionDelay { get; set; }

	public override void _Ready()
	{
		base._Ready();
		GenerateGrid();
	}

	public override void GenerateGrid()
	{
		InitializeGrid();
		var numberOfStartPoints = HowManyStartPoints();
		GenerateStartPoints(numberOfStartPoints);
		var active = Grid.QueryActiveCells();
		base.EmitGenerated();
		RunLife();
	}

	/// <summary>
	/// Runs the Life simulation for a specified number of life cycles.
	/// </summary>
	private async void RunLife()
	{
		for (int cycle = 0; cycle < LifeCycles; cycle++)
		{
			RunLifeCycle();
			
			if (CycleEmissionDelay > 0)
			{
				await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
				EmitSignal(MapGenerator.SignalName.MapUpdated, Grid);
			}
		}

		EmitSignal(MapGenerator.SignalName.MapFinalized, Grid);
	}

	/// <summary>
	/// Runs the life cycle of the game board.
	/// </summary>
	private void RunLifeCycle()
	{
		bool[,] lifeTracker = new bool[Grid.Size.X, Grid.Size.Y];

		// Assess the state of the board to judge what lives and dies
		for (int x = 0; x < Grid.Size.X; x++)
		{
			for (int y = 0; y < Grid.Size.Y; y++)
			{
				AssessCellLife(ref lifeTracker, new Vector2I(x, y));
			}
		}

		// Apply findings to the board.
		for (int x = 0; x < Grid.Size.X; x++)
		{
			for (int y = 0; y < Grid.Size.Y; y++)
			{
				Grid.GridCells[x,y].IsActive = lifeTracker[x,y];
			}
		}
	}

	/// <summary>
	/// Assess the life of a cell based on its neighbors and update the life tracker accordingly.
	/// </summary>
	/// <param name="position">The position of the cell to assess.</param>
	private void AssessCellLife(ref bool[,] lifeTracker, Vector2I position)
	{
		int x = position.X;
		int y = position.Y;
		bool isAlive = Grid.GridCells[x, y].IsActive;
		int aliveNeighbors = CountActiveNeighbors(x, y);
		if (isAlive)
		{
			if (aliveNeighbors < 2 || aliveNeighbors > 3)
			{
				lifeTracker[x, y] = false;
			}
			else
			{
				lifeTracker[x, y] = true;
			}
		}
		else
		{
			if (aliveNeighbors == 3)
			{
				lifeTracker[x, y] = true;
			}
			else
			{
				lifeTracker[x, y] = false;
			}
		}
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

	private int CountActiveNeighbors(int x, int y)
	{
		var activeCount = 0;
		var targetPosition = new Vector2I(x, y);
		if (!Grid.IsPositionSafe(targetPosition))
		{
			throw new ArgumentOutOfRangeException(nameof(targetPosition), "Position is not safe on the grid.");
		}

		Grid.MoveTo(targetPosition);
		var results = Grid.RelativeQuery(GetSurroundPattern());
		foreach (var result in results)
		{
			if (result.Value != null)
			{
				activeCount += (result.Value.IsActive) ? 1 : 0;
			}
		}

		return activeCount;
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

	private HashSet<GeneratorGrid.Direction> GetSurroundPattern()
	{
		return new HashSet<GeneratorGrid.Direction>
		{
			GeneratorGrid.Direction.North,
			GeneratorGrid.Direction.NorthEast,
			GeneratorGrid.Direction.East,
			GeneratorGrid.Direction.SouthEast,
			GeneratorGrid.Direction.South,
			GeneratorGrid.Direction.SouthWest,
			GeneratorGrid.Direction.West,
			GeneratorGrid.Direction.NorthWest,
		};
	}
} 
