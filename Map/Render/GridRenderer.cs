using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot.Collections;
using Roguelike.Map.Generator;
using Roguelike.Map.Model;
using FileAccess = Godot.FileAccess;

namespace Roguelike.Map.Render;

public partial class GridRenderer : Node
{
	/// <summary>
	/// Gets the dictionary that contains the tile type assignments.
	/// </summary>
	/// <value>
	/// The dictionary that contains the tile type assignments.
	/// </value>
	public System.Collections.Generic.Dictionary<TileType, HashSet<TileAddress>> TileTypeAssignments { get; private set; }
	
	/// <summary>
	/// @stub
	/// Reads tile associations from a JSON file.
	/// </summary>
	public GridRenderer(Array<MapGenerator> mapGenerators, TileSet sourceTileSet, string tileAssociationsPath )
	{
		if (mapGenerators.Count == 0 || sourceTileSet == null || string.IsNullOrEmpty(tileAssociationsPath))
		{
			throw new InvalidOperationException(
				"The active map generator, source tileset, or tile associations path is missing.");
		}

		TileTypeAssignments = new System.Collections.Generic.Dictionary<TileType, HashSet<TileAddress>>();

		Json json = loadAndExtractAssociationJson(tileAssociationsPath);
		
		var data = json.Data;
		Godot.Collections.Dictionary dictionary = data.AsGodotDictionary();
		ValidateTopLevelAssociationJsonFile(dictionary);
		
		Godot.Collections.Array tileTypeAssociations = dictionary["tiletype_associations"].AsGodotArray();

		// Group up all tile types for validation.
		TileTypeList allTileTypes = new TileTypeList();
		foreach (MapGenerator generator in mapGenerators)
		{
			foreach (TileType tiletype in generator.TileTypes)
			{
				if (!allTileTypes.Contains(tiletype))
				{
					allTileTypes.Add(tiletype);
				}
			}
		}
		
		for (int i = 0; i < tileTypeAssociations.Count ; i++)
		{
			Godot.Collections.Dictionary tileTypeAssociation = tileTypeAssociations[i].AsGodotDictionary();
			ValidateAssociationStructureInJsonFile(tileTypeAssociation) ;
			
			string tileTypeName = tileTypeAssociation["tiletype"].AsString();
			TileType tileType = allTileTypes.FindByName(tileTypeName);
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

	/// <summary>
	/// Renders the given generator grid by assigning tiles to grid cells based on their type.
	/// </summary>
	/// <param name="grid">The generator grid to render.</param>
	/// <exception cref="InvalidOperationException">
	/// Thrown if the type of a grid cell is not found in the TileTypeAssignments dictionary.
	/// Thrown if no tiles are available for a grid cell's tile type in the TileTypeAssignments dictionary.
	/// </exception>
	public void RenderGrid(GeneratorGrid grid, TileMap ActiveTileMap)
	{
		TileType currentTileType;
		HashSet<TileAddress> currentTilesAvailable;
		for (int x = 0; x < grid.Size.X ; x++)
		{
			for (int y = 0; y < grid.Size.Y; y++)
			{
				GridCell cell = grid.GridCells[x, y];

				if (!cell.IsActive)
				{
					ActiveTileMap.EraseCell(0, new Vector2I(x,y));
					continue;
				}
				
				currentTileType = grid.GridCells[x, y].Type;
				if (currentTileType == null)
				{
					throw new InvalidOperationException("Type (TileType) not found on Grid Cell [" + x.ToString() +
														"," + y.ToString() + "]");
				}
				
				currentTilesAvailable = TileTypeAssignments[currentTileType];
				if (currentTilesAvailable == null)
				{
					throw new InvalidOperationException("Invalid TileType on Type property of Grid Cell [" + x.ToString() + "," + y.ToString() + "]");
				}

				int tileListLength = currentTilesAvailable.Count;
				if (tileListLength == 0)
				{
					throw new InvalidOperationException("No TileSet tiles are avilable for TileType " + currentTileType.Name);
				}
				int randomIndex = new Random().Next(tileListLength);
				TileAddress randomTile = currentTilesAvailable.ElementAt(randomIndex);
				
				ActiveTileMap.SetCell(0, new Vector2I( x, y ), randomTile.AtlasId, randomTile.Position );	
			}	
		}	
	}


	private Json loadAndExtractAssociationJson(string tileAssociationsPath)
	{
		Json json = new Json();
		Godot.FileAccess file = FileAccess.Open(tileAssociationsPath, FileAccess.ModeFlags.Read);
		if (file == null)
		{
			throw new FileNotFoundException("The specified json file could not be found or loaded.", tileAssociationsPath);	
		}
		Godot.Error error = json.Parse(file.GetAsText());
		file.Close();

		if (error != Godot.Error.Ok)
		{
			throw new IOException("An error occurred while parsing the json file.");	
		}

		return json;
	}
	
	private void ValidateTopLevelAssociationJsonFile(Dictionary dictionary)
	{
		if (!dictionary.ContainsKey("tiletype_associations"))
		{
			throw new InvalidOperationException("Required key 'tiletype_associations' is missing from the source json file");
		}	
	}

	private void ValidateAssociationStructureInJsonFile(Dictionary dictionary)
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

	private void ValidateTileAddressJson(Dictionary dictionary)
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
