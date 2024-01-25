using Godot;
using Godot.Collections;
using Roguelike.Map.Model;
using Roguelike.Map.Model.Shapes;

namespace Roguelike.Map.Generator;

public abstract partial class MapGenerator : Godot.Node
{
	/// <summary>
	/// When defined, any work done by this generator will only take place within SelectionZones.
	/// Area outside of SelectionZone will be unaffected by this generator.
	/// If no SelectionZones are supplied, work will happen across entire map.
	/// </summary>
	[Export] public Array<Shape> SelectionZones { get; set; }
	
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
	/// The width value represents the width dimension of the map being mainpulated, in 'squares.' 
	/// </remarks>
	public int Width { get; private set; }
	
	/// <summary>
	/// Gets the height of the Map being generated.
	/// </summary>
	/// <remarks>
	/// The width value represents the height dimension of the map being manipulated, in 'sqaures.' 
	/// </remarks>
	public int Height { get; private set; }

	/// <summary>
	/// Gets the list of available tile types used by this generator.
	/// </summary>
	public TileTypeList TileTypes { get; private set; }

	private GeneratorGrid _grid;
	/// <summary>
	/// Represents a generator grid (the grid we'll be working with to abstract our procedural genration work).
	/// </summary>
	public Model.GeneratorGrid Grid
	{
		get { return _grid; }
		set
		{
			_grid = value;

			if (_grid != null)
			{
				Width = _grid.Size.X;
				Height = _grid.Size.Y;
			}
		}
	}

	public MapGenerator()
	{
		TileTypes = new TileTypeList();
	}

	/// <summary>
	/// Generate the procedural grid.
	/// </summary>
	public abstract void Begin();
}
