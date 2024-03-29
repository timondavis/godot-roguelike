using System;
using Godot;
using Godot.Collections;
using Roguelike.Map.Generator.Service;
using Roguelike.Map.Model.Grid;
using Roguelike.Map.Render;

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

	/// <summary>
	/// Gets or sets the width of the generated map.
	/// </summary>
	/// <remarks>
	/// The generated map width determines the horizontal size of the map that is being generated.
	/// </remarks>
	/// <value>
	/// The width of the generated map.
	/// </value>
	[Export]
	public int GeneratedMapWidth { get; set; }

	/// <summary>
	/// Gets or sets the height of the generated map.
	/// </summary>
	/// <remarks>
	/// This property represents the height of the generated map.
	/// The value of this property is used to determine the vertical size of the generated map.
	/// </remarks>
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
	public MapGenerator ActiveMapGenerator { get; private set; }
	private int _activeMapGeneratorIndex = 0;
	private GridRenderer _gridRenderer;
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (MapGeneratorSequence.Count == 0)
		{
			throw new InvalidOperationException("MapGeneratorQueue must have at least one element.");
		}

		// Initialize the Grid Renderer.
		_gridRenderer = new GridRenderer(MapGeneratorSequence, TileAssociationsPath);
		
		// Assign the given TileSet to the ActiveTileMap.  
		ActiveTileMap.TileSet = SourceTileSet;
		
		// Reset (Initialize) the GeneratorGrid.
		ResetGrid();

		// Set the 0th ActiveMapGenerator in the MapGeneratorSequence array.  Connect it to the system.
		ActiveMapGenerator = MapGeneratorSequence[_activeMapGeneratorIndex];
		ConnectActiveMapGenerator();
		
		// Start work on the first generator (Subsequent generators will be invoked on the OnMapFinalized callback).
		ActiveMapGenerator.Begin();
	}
	
	/// <summary>
	/// Reinitialize the Grid, clearing out any contents.
	/// </summary>
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
			ActiveMapGenerator = MapGeneratorSequence[++_activeMapGeneratorIndex];
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
		
		// Register any SelectedAreas indicated on the MapGenerator to the SelectionService
		if (ActiveMapGenerator.SelectedAreas.Count > 0)
		{
			SelectionService.Instance.SelectedAreas = ActiveMapGenerator.SelectedAreas;
		}
		
		// Attach Grid to Generator
		ActiveMapGenerator.Grid = Grid;
	}

	private void DisconnectActiveMapGenerator()
	{
		// Remove Connects to ActiveMapGenerator Delegates. 
		ActiveMapGenerator.MapGenerated -= OnMapGenerated;
		ActiveMapGenerator.MapUpdated -= OnMapUpdated;
		ActiveMapGenerator.MapFinalized -= OnMapFinalized;
		
		// Clear any selected zones
		SelectionService.Instance.ClearSelectedAreas();

		// Disconnect the grid from the active map generator.
		ActiveMapGenerator.Grid = null;
	}

	private GeneratorGrid InitializeGrid()
	{
		if (GeneratedMapWidth > 0 && GeneratedMapHeight > 0)
		{
			GeneratorGrid grid;
			if (ActiveTileMap.TileSet.TileShape == TileSet.TileShapeEnum.Square)
			{
				grid = new SquareGeneratorGrid(new Vector2I( GeneratedMapWidth, GeneratedMapHeight));
			} else if (ActiveTileMap.TileSet.TileShape == TileSet.TileShapeEnum.Hexagon)
			{
				grid = new HexGeneratorGrid(new Vector2I(GeneratedMapWidth, GeneratedMapHeight));
			}
			else
			{
				throw new ArgumentException(
					"Tileset must be Square or Hexagon based.  More shapes may be supported in the future.");
			}

			return grid;
		}
		else
		{
			throw new ArgumentOutOfRangeException("Width and Height must be set before initializing " +
												  "MapGenerator's Generator Grid.");
		}
	}
}
