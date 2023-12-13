using Godot;
using Roguelike.Map.Model;

namespace Roguelike.Map;

public partial class GeneratedMap : TileMap
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TileSetSource source = TileSet.GetSource(1);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnMapGenerated(GeneratorGrid grid)
	{
		var cells = grid.GridCells;
		for (int x = 0; x < grid.Size.X ; x++)
		{
			for (int y = 0; y < grid.Size.Y; y++)
			{
				if (grid.GridCells[x, y].IsActive)
				{
					SetCell(0, new Vector2I( x, y ), 0, new Vector2I(3, 0) );	
				}
			}	
		}
	}
}
