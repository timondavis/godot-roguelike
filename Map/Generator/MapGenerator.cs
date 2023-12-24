using System;
using Godot;
using Roguelike.Map.Model;

namespace Roguelike.Map.Generator;

public abstract partial class MapGenerator : Godot.Node
{
	/// <summary>
	/// Delegate representing an event when a map is generated.
	/// </summary>
	/// <param name="grid">The generated grid for the map.</param>
	[Signal]
	public delegate void MapGeneratedEventHandler(Model.GeneratorGrid grid);

	/// <summary>
	/// A delegate representing the event handler for when the map is updated. </summary> <param name="grid">The updated generator grid.</param>
	/// /
	[Signal]
	public delegate void MapUpdatedEventHandler(Model.GeneratorGrid grid);

	/// <summary>
	/// Delegate to handle the event when a map has been finalized.
	/// </summary>
	/// <param name="grid">The finalized generator grid.</param>
	[Signal]
	public delegate void MapFinalizedEventHandler(Model.GeneratorGrid grid);

	/// <summary>
	/// Gets the width of the Map being generated.
	/// </summary>
	/// <remarks>
	/// The width value represents the width dimension of the map being generated, in 'sqaures'.
	/// </remarks>
	[Export]
	public int Width { get; private set; }
	
	/// <summary>
	/// Gets the height of the Map being generated.
	/// </summary>
	/// <remarks>
	/// The width value represents the height dimension of the map being generated, in 'sqaures'.
	/// </remarks>
	[Export]
	public int Height { get; private set; }

	/// <summary>
	/// Gets the list of available tile types used by this generator.
	/// </summary>
	public TileTypeList TileTypes { get; private set; }

	/// <summary>
	/// Represents a generator grid (the grid we'll be working with to abstract our procedural genration work).
	/// </summary>
	public Model.GeneratorGrid Grid;

	/// <summary
	public MapGenerator()
	{
		TileTypes = new TileTypeList();
	}

	/// <summary>
	/// Generate the procedural grid.
	/// </summary>
	public abstract void GenerateGrid();

	/// <summary>
	/// Initializes the grid for the MapGenerator.
	/// </summary>
	/// <exception cref="ArgumentOutOfRangeException">Thrown if Width or Height is less than or equal to zero.</exception>
	public void InitializeGrid()
	{
		if (Width > 0 && Height > 0)
		{
			Grid = new Model.GeneratorGrid(new Vector2I(Width, Height));
		}
		else
		{
			throw new ArgumentOutOfRangeException("Width and Height must be set before initializing " +
												  "MapGenerator's Generator Grid.");
		}
	}
}
