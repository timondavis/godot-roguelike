using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using Roguelike.Map.Model;
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

		Json json = loadAndExtractAssociationJson();
		
		var data = json.Data;
		Godot.Collections.Dictionary dictionary = data.AsGodotDictionary();
		ValidateTopLevelAssociationJsonFile(dictionary);
		
		Godot.Collections.Array tileTypeAssociations = dictionary["tiletype_associations"].AsGodotArray();
		
		for (int i = 0; i < tileTypeAssociations.Count ; i++)
		{
			Godot.Collections.Dictionary tileTypeAssociation = tileTypeAssociations[i].AsGodotDictionary();
			ValidateAssociationStructureInJsonFile(tileTypeAssociation) ;
			
			string tileTypeName = tileTypeAssociation["tiletype"].AsString();
			TileType tileType = ActiveMapGenerator.TileTypes.FindByName(tileTypeName);
			VerifyJsonIsRequestingValidTileType(tileTypeName, tileType);

			Godot.Collections.Array tileAddressArray = tileTypeAssociation["association"].AsGodotArray();

			for (int j = 0; j < tileAddressArray.Count; j++)
			{
				Godot.Collections.Dictionary tileAddressDictionary = tileAddressArray[j].AsGodotDictionary();
				ValidateTileAddressJson(tileAddressDictionary);

				TileAddress tileAddress = new TileAddress(
					tileAddressDictionary["atlasId"].AsInt32(),
					tileAddressDictionary["atlasX"].AsInt32(),
					tileAddressDictionary["atlasY"].AsInt32()
				);

				if (!TileTypeAssignments.ContainsKey(tileType))
				{
					TileTypeAssignments.Add(tileType, new HashSet<TileAddress>());
				}

				HashSet<TileAddress> writeToHashSet;
				TileTypeAssignments.TryGetValue(tileType, out writeToHashSet);
				writeToHashSet.Add(tileAddress);
			}
		}
	}

	private Json loadAndExtractAssociationJson()
	{
		Json json = new Json();
		Godot.FileAccess file = FileAccess.Open(TileAssociationsPath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			throw new FileNotFoundException("The specified json file could not be found or loaded.", TileAssociationsPath);	
		}
		Godot.Error error = json.Parse(file.GetAsText());
		file.Close();

		if (error != Godot.Error.Ok)
		{
			throw new IOException("An error occurred while parsing the json file.");	
		}

		return json;
	}

	private void ValidateTopLevelAssociationJsonFile(Godot.Collections.Dictionary dictionary)
	{
		if (!dictionary.ContainsKey("tiletype_associations"))
		{
			throw new InvalidOperationException("Required key 'tiletype_associations' is missing from the source json file");
		}	
	}

	private void ValidateAssociationStructureInJsonFile(Godot.Collections.Dictionary dictionary)
	{
		// Check for required property names
		if (!dictionary.ContainsKey("tiletype"))
		{
			throw new InvalidOperationException("Required key 'tiletype' is missing from the source json file");
		}

		if (!dictionary.ContainsKey("association"))
		{
			throw new InvalidOperationException("Required key 'association' is missing from the source json file");
		}	
	}

	private void VerifyJsonIsRequestingValidTileType(string tileTypeName, TileType tileType)
	{
		if (tileType == null)
		{
			throw new InvalidOperationException("TileType '" + tileTypeName +
			                                    "' was specified in source json, but does not exist on ActiveMapGenerator.");
		}	
	}

	private void ValidateTileAddressJson(Godot.Collections.Dictionary dictionary)
	{
		// Throw error if required keys not found
		if (!dictionary.ContainsKey("atlasId"))
		{
			throw new InvalidOperationException("Required key 'atlasId' is missing from the source json file");
		}
		if (!dictionary.ContainsKey("atlasX"))
		{
			throw new InvalidOperationException("Required key 'atlasX' is missing from the source json file");
		}
		if (!dictionary.ContainsKey("atlasY"))
		{
			throw new InvalidOperationException("Required key 'atlasY' is missing from the source json file");
		}	
	}
}
