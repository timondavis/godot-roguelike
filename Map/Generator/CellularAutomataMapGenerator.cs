using System;
using System.Collections.Generic;
using Godot;
using Roguelike.Map.Generator.Service;
using Roguelike.Map.Model.Direction;
using Roguelike.Map.Model.Direction.Pattern;
using Roguelike.Map.Model.Grid;

namespace Roguelike.Map.Generator;

public partial class CellularAutomataMapGenerator : Roguelike.Map.Generator.MapGenerator
{
	public const string TileType_Floor = "floor";

	[Export(PropertyHint.Range, "0.01, 1.00, 0.01")]
	public float StartingDensity;

	[Export(PropertyHint.Range, "1, 1000, 1)")]
	public int LifeCycles;

	[Export] public float CycleEmissionDelay;

	// Settings for life generation and sustainance. By default, implements Game of Life.
	[Export] 
	public int MinNeighborsForSustainedLife = 2;
	[Export] 
	public int MaxNeighborsForSustainedLife = 3;
	[Export] 
	public int MinNeighborsForNewLife = 3;
	[Export] 
	public int MaxNeighborsForNewLife = 3;
	
	private DirectionalPatternFactory _patternFactory = new DirectionalPatternFactory();

	public CellularAutomataMapGenerator() : base()
	{
		TileTypes.Add(new Model.TileType { Name=TileType_Floor } );
	}
	

	/// <summary>
	/// Generates a grid for the map.
	/// </summary>
	public override void Begin()
	{
		var numberOfStartPoints = HowManyStartPoints();
		GenerateStartPoints(numberOfStartPoints);
		var active = Grid.QueryActiveCells();
		EmitSignal(Roguelike.Map.Generator.MapGenerator.SignalName.MapGenerated, Grid);
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
				EmitSignal(Roguelike.Map.Generator.MapGenerator.SignalName.MapUpdated, Grid);
			}
		}

		EmitSignal(Roguelike.Map.Generator.MapGenerator.SignalName.MapFinalized, Grid);
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
				var position = new Vector2I(x, y);
				if (SelectionService.Instance.IsPositionSelected(position))
				{
					AssessCellLife(ref lifeTracker, position);
				}
			}
		}

		// Apply findings to the board.
		for (int x = 0; x < Grid.Size.X; x++)
		{
			for (int y = 0; y < Grid.Size.Y; y++)
			{
				if (lifeTracker[x, y])
				{
					Grid.GridCells[x,y].Activate( TileTypes.FindByName(TileType_Floor) );
				}
				else
				{
					Grid.GridCells[x,y].Deactivate();
				}
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
			if (aliveNeighbors >= MinNeighborsForSustainedLife && aliveNeighbors <= MaxNeighborsForSustainedLife )
			{
				lifeTracker[x, y] = true;
			}
			else
			{
				lifeTracker[x, y] = false;
			}
		}
		else
		{
			if (aliveNeighbors >= MinNeighborsForNewLife && aliveNeighbors <= MaxNeighborsForNewLife)
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

		HashSet<GridDirection> starPattern = 
			_patternFactory.Generate(Grid.TileShape, DirectionalPatternEnum.Star).Pattern;
		HashSet<GridDirection> plusPattern =
			_patternFactory.Generate(Grid.TileShape, DirectionalPatternEnum.Plus).Pattern;
		
		// Just in case we can't find any more unique values, set a threshold and count failed attempts
		// To generate Unique values.
		GD.Randomize();
		for (int i = 0; i < howManyPoints ; i++) 
		{
			x = (int)GD.RandRange(0, Width);
			y = (int)GD.RandRange(0, Height);
			Grid.MoveTo(new Vector2I(x, y));

			Godot.Collections.Dictionary<GridDirection, Model.GridCell> cells;

			if (i % 2 == 0)
			{
				cells = Grid.RelativeQuery( 
					_patternFactory.Generate(Grid.TileShape, DirectionalPatternEnum.Star) 
				);
			}
			else
			{
				cells = Grid.RelativeQuery( 
					_patternFactory.Generate(Grid.TileShape, DirectionalPatternEnum.Plus)
				);
			}
			
			foreach (var cell in cells)
			{
				if (cell.Value != null)
				{
					cell.Value.Activate(TileTypes.FindByName(TileType_Floor));
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
		var results = Grid.RelativeQuery(
			_patternFactory.Generate(
				Grid.TileShape, DirectionalPatternEnum.Surround)
			);
		foreach (var result in results)
		{
			if (result.Value != null)
			{
				activeCount += (result.Value.IsActive) ? 1 : 0;
			}
		}

		return activeCount;
	}
}
