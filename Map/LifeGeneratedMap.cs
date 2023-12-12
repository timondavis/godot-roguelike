using Godot;
using System;

public partial class LifeGeneratedMap : TileMap
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TileSetSource source = TileSet.GetSource(1);
		SetCell(0, new Vector2I( 0, 0 ), 0, new Vector2I(0, 0) );
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
