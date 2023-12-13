using System;
using Godot;
using Roguelike.Map.Model;

namespace Roguelike.Map.Generator;

public partial class MapGenerator : Node
{
	
	[Export] 
	public int Width { get; set; }
	
	[Export] 
	public int Height { get; set; }

	[Signal]
	public delegate void MapGeneratedEventHandler(GeneratorGrid grid);

	[Signal]
	public delegate void MapUpdatedEventHandler(GeneratorGrid grid);
	
	public GeneratorGrid Grid;
	
	public virtual void GenerateGrid()
	{
		if (Width > 0 && Height > 0)
		{
			InitializeGrid();
		}

		EmitSignal(MapGenerator.SignalName.MapGenerated, Grid);
	}

	public void InitializeGrid()
	{
		if (Width > 0 && Height > 0)
		{
			Grid = new GeneratorGrid(new Vector2I(Width, Height));
		}
		else
		{
			throw new ArgumentOutOfRangeException("Width and Height must be set before initializing " +
												  "MapGenerator's Generator Grid.");
		}
	}
}
