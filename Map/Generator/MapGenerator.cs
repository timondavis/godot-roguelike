using System;
using Godot;
using System.Collections;
using System.Collections.Generic;
using Roguelike.Map.Model;

namespace Roguelike.Map.Generator;

public abstract partial class MapGenerator : Node
{
	
	[Signal]
	public delegate void MapGeneratedEventHandler(GeneratorGrid grid);

	[Signal]
	public delegate void MapUpdatedEventHandler(GeneratorGrid grid);

	[Signal]
	public delegate void MapFinalizedEventHandler(GeneratorGrid grid);
	
	public int Width { get; private set; }
	
	public int Height { get; private set; }

	public TileTypeList TileTypes { get; private set; }


	public GeneratorGrid Grid;

	public MapGenerator(int width, int height)
	{
		Width = width;
		Height = height;
		TileTypes = new TileTypeList();
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
