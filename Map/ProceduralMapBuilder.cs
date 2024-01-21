using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Godot.Collections;
using Roguelike.Map.Model;
using Roguelike.Map.Render;
using FileAccess = Godot.FileAccess;

namespace Roguelike.Map.Generator;

/// <summary>
/// ProceduralTileMapGenerator class is responsible for generating and rendering a procedural tile map using a given map generator and tile set.
/// </summary>
public partial class ProceduralMapBuilder : Node
{
	[Export] 
	//  The Active Tile Map to be mainipulated
	public TileMap ActiveTileMap { get; set; }

	[Export] 
	// The TileSet from which tiles will be extracted and applied to the TileMap
	public TileSet SourceTileSet { get; set; }

	[Export]
	public int GeneratedMapWidth { get; set; }
	
	[Export]
	public int GeneratedMapHeight { get; set; }

	[Export] public Array<Roguelike.Map.Generator.MapGenerator> MapGeneratorSequence { get; set; }
	
	[Export(PropertyHint.File, "*.json")] 
	// Path to JSON file which describes the relationships between the Tiles found in a TileSet
	// and their corresponding TileTypes, as provided by the MapGenerator.
	public string TileAssociationsPath { get; set; }

	/// <summary>
	/// Represents the primary enerator grid.
	/// </summary>
	public GeneratorGrid Grid;
	
	// The Map Generator Instance which is responsible for providing the procedural genration algorithm which
	// will be used to build the map.
	public Roguelike.Map.Generator.MapGenerator ActiveMapGenerator { get; private set; }
	private int _activeMapGeneratorIndex = 0;
	private GridRenderer _gridRenderer;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (MapGeneratorSequence.Count == 0)
		{
			throw new InvalidOperationException("MapGeneratorQueue must have at least one element.");
		}

		_gridRenderer = new GridRenderer(MapGeneratorSequence, SourceTileSet, TileAssociationsPath);
		ResetGrid();

		ActiveMapGenerator = MapGeneratorSequence[_activeMapGeneratorIndex];
		ConnectActiveMapGenerator();

		ActiveTileMap.TileSet = SourceTileSet;
		ActiveMapGenerator.Begin();
	}
	
	public void ResetGrid()
	{
		Grid = InitializeGrid();
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
		DisconnectActiveMapGenerator();
	}

	/// <summary>
	/// Executes when a map is first generated.  Should only be called exactly once, as the first signal,
	/// from MapGenerator instance.
	/// </summary>
	/// <param name="grid">The generated grid.</param>
	private void OnMapGenerated(GeneratorGrid grid)
	{
		_gridRenderer.RenderGrid(grid, ActiveTileMap);
	}

	/// <summary>
	/// Handles the event when the map is updated.  May be signalled multiple times.
	/// </summary>
	/// <param name="grid">The updated generator grid.</param>
	private void OnMapUpdated(GeneratorGrid grid)
	{
		OnMapGenerated(grid);
	}

	/// <summary>
	/// The method is called when the map generation process has been finalized.  Should be signalled exactly once.
	/// It triggers the OnMapGenerated event passing the generated grid as a parameter.
	/// </summary>
	/// <param name="grid">The generated grid object.</param>
	private void OnMapFinalized(GeneratorGrid grid)
	{
		OnMapGenerated(grid);
		if (TryAdvanceToNextMapGenerator())
		{
			ActiveMapGenerator.Begin();
		}
	}

	private bool TryAdvanceToNextMapGenerator()
	{
		DisconnectActiveMapGenerator();
		if (_activeMapGeneratorIndex + 1 < MapGeneratorSequence.Count)
		{
			ActiveMapGenerator = MapGeneratorSequence[_activeMapGeneratorIndex];
			ConnectActiveMapGenerator();
			return true;
		}

		return false;
	}
	
	private void ConnectActiveMapGenerator()
	{
		// Attach callbacks to ActiveMapGenerator Delegates.
		ActiveMapGenerator.MapGenerated += OnMapGenerated;
		ActiveMapGenerator.MapUpdated += OnMapUpdated;
		ActiveMapGenerator.MapFinalized += OnMapFinalized;	
		
		// Attach Grid to Generator
		ActiveMapGenerator.Grid = Grid;
	}

	private void DisconnectActiveMapGenerator()
	{
		// Remove Connects to ActiveMapGenerator Delegates. 
		ActiveMapGenerator.MapGenerated -= OnMapGenerated;
		ActiveMapGenerator.MapUpdated -= OnMapUpdated;
		ActiveMapGenerator.MapFinalized -= OnMapFinalized;

		ActiveMapGenerator.Grid = null;
	}

	private GeneratorGrid InitializeGrid()
	{
		if (GeneratedMapWidth > 0 && GeneratedMapHeight > 0)
		{
			return new GeneratorGrid(new Vector2I( GeneratedMapWidth, GeneratedMapHeight));
		}
		else
		{
			throw new ArgumentOutOfRangeException("Width and Height must be set before initializing " +
												  "MapGenerator's Generator Grid.");
		}
	}
}
