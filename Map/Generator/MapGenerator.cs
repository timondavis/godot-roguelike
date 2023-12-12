using System;
using Godot;
using Roguelike.Map.Model;

namespace Roguelike.Map.Generator;

public abstract class MapGenerator : Node
{
	public GeneratorGrid Grid;
	public abstract void GenerateGrid();

	[Export] 
	public int Width { get; set; }
	
	[Export] 
	public int Height { get; set; }

	public void InitializeGrid()
	{
		if (Width >= 0 && Height >= 0)
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