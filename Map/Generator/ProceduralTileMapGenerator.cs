using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Roguelike.Map.Generator;

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
	public Roguelike.Map.Generator.MapGenerator ActiveMapGenerator { get; set; }
	
	[Export(PropertyHint.File, "*.json")] 
	// Path to JSON file which describes the relationships between the Tiles found in a TileSet
	// and their corresponding TileTypes, as provided by the MapGenerator.
	public string TileAssociationsPath { get; set; }

	/// <summary>
	/// Gets the dictionary that contains the tile type assignments.
	/// </summary>
	/// <value>
	/// The dictionary that contains the tile type assignments.
	/// </value>
	public Dictionary<Model.TileType, HashSet<TileAddress>> TileTypeAssignments { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Attach callbacks to ActiveMapGenerator Delegates.
		ActiveMapGenerator.MapGenerated += OnMapGenerated;
		ActiveMapGenerator.MapUpdated += OnMapUpdated;
		ActiveMapGenerator.MapFinalized += OnMapFinalized;

		TileTypeAssignments = new Dictionary<Model.TileType, HashSet<TileAddress>>();

		ReadTileAssociations();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	/// <summary>
	/// This method is invoked when the node is about to be removed from the scene tree.
	/// It is responsible for cleaning up any resources or event subscriptions.
	/// </summary>
	public override void _ExitTree()
	{
		// Remove Connects to ActiveMapGenerator Delegates. 
		ActiveMapGenerator.MapGenerated -= OnMapGenerated;
		ActiveMapGenerator.MapUpdated -= OnMapUpdated;
		ActiveMapGenerator.MapFinalized -= OnMapFinalized;	
	}

	private void OnMapGenerated(Model.GeneratorGrid grid)
	{
	}

	private void OnMapUpdated(Model.GeneratorGrid grid)
	{
	}

	private void OnMapFinalized(Model.GeneratorGrid grid)
	{
	}

	/// <summary>
	/// @stub
	/// Reads tile associations from a JSON file.
	/// </summary>
	private void ReadTileAssociations()
	{
		if (ActiveMapGenerator == null || SourceTileSet == null || string.IsNullOrEmpty(TileAssociationsPath))
		{
			throw new InvalidOperationException(
				"The active map generator, source tileset, or tile associations path is missing.");
		}

		var json = new Json();
		var file = FileAccess.Open(TileAssociationsPath, FileAccess.ModeFlags.Read);
		Godot.Error error = json.Parse(file.GetAsText());
		file.Close();

		if (error == Godot.Error.Ok)
		{
			var data = json.Data;
		}
		else
		{
			// Handle exception and terminate load.
		}






		/*
		var jsonContent = File.ReadAllText(TileAssociationsPath);
		var jsonData = JsonDocument.Parse(jsonContent);
var a = 1;
		*/

		// .. with Json Data, you can now inform the appropriate data structure..
	}
}
