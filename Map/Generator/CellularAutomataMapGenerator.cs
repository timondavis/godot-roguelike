using System;
using System.Collections.Generic;
using Godot;

namespace Roguelike.Map.Generator;

public partial class CellularAutomataMapGenerator : Roguelike.Map.Generator.MapGenerator
{
	public const string TileType_Floor = "floor";
	
	[Export(PropertyHint.Range, "0.01, 1.00, 0.01" )] 
	public float StartingDensity { get; set; }
	
	[Export( PropertyHint.Range, "1, 1000, 1)" )]
	public int LifeCycles { get; set; }
	
	[Export]
	public float CycleEmissionDelay { get; set; }

	public CellularAutomataMapGenerator() : base()
	{
		TileTypes.Add(new Model.TileType { Name=TileType_Floor } );
	}

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
				AssessCellLife(ref lifeTracker, new Vector2I(x, y));
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

		HashSet<Model.GeneratorGrid.Direction> starPattern = GetStarPattern();
		HashSet<Model.GeneratorGrid.Direction> plusPattern = GetPlusPattern();
		
		// Just in case we can't find any more unique values, set a threshold and count failed attempts
		// To generate Unique values.
		GD.Randomize();
		for (int i = 0; i < howManyPoints ; i++) 
		{
			x = (int)GD.RandRange(0, Width);
			y = (int)GD.RandRange(0, Height);
			Grid.MoveTo(new Vector2I(x, y));

			Godot.Collections.Dictionary<Model.GeneratorGrid.Direction, Model.GridCell> cells;

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

	private HashSet<Model.GeneratorGrid.Direction> GetStarPattern()
	{
		return new HashSet<Model.GeneratorGrid.Direction>
		{
			Model.GeneratorGrid.Direction.Here,
			Model.GeneratorGrid.Direction.NorthWest,
			Model.GeneratorGrid.Direction.NorthEast,
			Model.GeneratorGrid.Direction.SouthWest,
			Model.GeneratorGrid.Direction.SouthEast,
		}; 
	}

	private HashSet<Model.GeneratorGrid.Direction> GetPlusPattern()
	{
		return new HashSet<Model.GeneratorGrid.Direction>
		{
			Model.GeneratorGrid.Direction.Here,
			Model.GeneratorGrid.Direction.East,
			Model.GeneratorGrid.Direction.South,
			Model.GeneratorGrid.Direction.West,
			Model.GeneratorGrid.Direction.North
		}; 
	}

	private HashSet<Model.GeneratorGrid.Direction> GetSurroundPattern()
	{
		return new HashSet<Model.GeneratorGrid.Direction>
		{
			Model.GeneratorGrid.Direction.North,
			Model.GeneratorGrid.Direction.NorthEast,
			Model.GeneratorGrid.Direction.East,
			Model.GeneratorGrid.Direction.SouthEast,
			Model.GeneratorGrid.Direction.South,
			Model.GeneratorGrid.Direction.SouthWest,
			Model.GeneratorGrid.Direction.West,
			Model.GeneratorGrid.Direction.NorthWest,
		};
	}

}
