using System;
using Godot;
using System.Collections;
using System.Collections.Generic;
using Roguelike.Map.Model;

namespace Roguelike.Map.Generator;

public abstract partial class MapGenerator : Node
{
	
	[Export] 
	public int Width { get; set; }
	
	[Export] 
	public int Height { get; set; }

	[Signal]
	public delegate void MapGeneratedEventHandler(GeneratorGrid grid);

	[Signal]
	public delegate void MapUpdatedEventHandler(GeneratorGrid grid);

	[Signal]
	public delegate void MapFinalizedEventHandler(GeneratorGrid grid);
	
	public GeneratorGrid Grid;
	
	protected void EmitGenerated()
	{
		EmitSignal(SignalName.MapGenerated, Grid);
	}
	
	public abstract void GenerateGrid();

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
