using Godot;
using System;
using System.IO;
using System.Text.Json;
using Roguelike.Map.Generator;
using Roguelike.Map.Model;

public partial class ProceduralTileMapGenerator : Node
{
	[Export] 
	//  The Active Tile Map to be mainipulated
	public TileMap ActiveTileMap { get; set; }

	[Export] 
	// The TileSet from which tiles will be extracted and applied to the TileMap
	public TileSet SourceTileSet { get; set; }

	[Export] 
	// The Map Generator Instance which is repsponsible for providing the procedural genration algorithm which
	// will be used to build the map.
	public MapGenerator ActiveMapGenerator { get; set; }
	
	[Export(PropertyHint.File, "*.json")] 
	// Path to JSON file which describes the relationships between the Tiles found in a TileSet
	// and their corresponding TileTypes, as provided by the MapGenerator.
	public string TileAssociationsPath { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Attach callbacks to ActiveMapGenerator Delegates.
		ActiveMapGenerator.MapGenerated += OnMapGenerated;
		ActiveMapGenerator.MapUpdated += OnMapUpdated;
		ActiveMapGenerator.MapFinalized += OnMapFinalized;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _ExitTree()
	{
		// Remove Connects to ActiveMapGenerator Delegates. 
		ActiveMapGenerator.MapGenerated -= OnMapGenerated;
		ActiveMapGenerator.MapUpdated -= OnMapUpdated;
		ActiveMapGenerator.MapFinalized -= OnMapFinalized;	
	}

	private void OnMapGenerated(GeneratorGrid grid)
	{
	}

	private void OnMapUpdated(GeneratorGrid grid)
	{
	}

	private void OnMapFinalized(GeneratorGrid grid)
	{
	}

	/// <summary>
	/// @stub
	/// Reads tile associations from a JSON file.
	/// </summary>
	private void ReadTileAssociations()
	{
		if (!string.IsNullOrEmpty(TileAssociationsPath))
		{
			var jsonContent = File.ReadAllText(TileAssociationsPath);
			var jsonData = JsonDocument.Parse(jsonContent);
		}
		
		// .. with Json Data, you can now inform the appropriate data structure..
	}
}
