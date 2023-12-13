using Godot;
using System;
using Roguelike.Map.Generator;
using Roguelike.Map.Model;

public partial class LifeGeneratedMap : TileMap
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
			
	}

}

