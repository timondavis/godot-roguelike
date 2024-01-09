using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Roguelike.Map.Model;

namespace Roguelike.Map.Generator.Room;

public abstract partial class RoomGenerator : MapGenerator
{
   	public const string TileType_Floor = "floor";
    
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMin { get; set; }
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMax { get; set; }
	
	[Export] public float CycleEmissionDelay { get; set; }

	public int NumberOfRooms = 0;
	public int RoomApplicationAttemptsMax = 100;
	public HashSet<RectangleRoom> Rooms = new HashSet<RectangleRoom>();
	
	public RoomGenerator() : base()
	{
		TileTypes.Add(new TileType { Name=TileType_Floor } );
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		RoomCountMin = Math.Max(1, RoomCountMin);
		RoomCountMax = Math.Max(1, RoomCountMax);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void GenerateGrid()
	{
		if (RoomCountMax < RoomCountMin)
		{
			throw new ArgumentException("RoomCountMax cannot be less than RoomCountMin");
		}
		
		InitializeGrid();
		GD.Randomize();
		Generate();
	}

	/// <summary>
	/// Generates the map by randomly determining the number of rooms, placing the rooms on the map,
	/// connecting the rooms, and emitting the signal to mark map finalization.
	/// </summary>
	protected async void Generate()
	{
		NumberOfRooms = GD.RandRange(RoomCountMin, RoomCountMax);
		await PlaceRooms();
		await ConnectRooms();
		EmitSignal(SignalName.MapFinalized);	
	}

	/// <summary>
	/// Places the rooms in a specified location.
	/// </summary>
	/// <returns>A task representing the asynchronous operation.</returns>
	protected abstract Task PlaceRooms();

	/// <summary>
	/// Connects the rooms by finding a path between them and connecting each pair of adjacent rooms.
	/// </summary>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	protected virtual async Task ConnectRooms()
	{
		PathFinder pf = new PathFinder(Grid, TileTypes.FindByName(TileType_Floor));
		Queue<RectangleRoom> path = pf.FindRectangleRoomPath(Rooms);

		RectangleRoom room1;
		RectangleRoom room2;
		RoomConnector rc = new RoomConnector(Grid, TileTypes.FindByName(TileType_Floor));
		while (path.TryDequeue(out room1) && path.TryPeek(out room2))
		{
			rc.ConnectRooms(room1, room2);
			if (CycleEmissionDelay > 0)
			{
				await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
				EmitSignal(SignalName.MapUpdated, Grid);
			}
		}
	}

	/// <summary>
	/// Checks if a room is available.
	/// </summary>
	/// <param name="room">The room to check availability for.</param>
	/// <returns>True if the room is available; otherwise, false.</returns>
	/// <exception cref="NotImplementedException">The method is not implemented.</exception>
	protected bool IsRoomAvailable(Model.Room room)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Checks if a given room is available
	/// (ie it doesn't intersect with another existing active cell pattern) on the grid.
	/// </summary>
	/// <param name="room">The room to check availability for.</param>
	/// <returns>True if the room is available, false otherwise.</returns>
	protected bool IsRoomAvailable(RectangleRoom room)
	{
		Vector2I placeholder = new Vector2I(Grid.Current.Position.X, Grid.Current.Position.Y);
		for (int x = room.TopLeft.X; x < room.TopLeft.X + room.Size.X; x++)
		{
			for (int y = room.TopLeft.Y; y < room.TopLeft.Y + room.Size.Y; y++)
			{
				Grid.MoveTo(new Vector2I(x,y));
				if (Grid.Current.IsActive)
				{
					return false;
				};
			}
		}

		Grid.MoveTo(placeholder);
		return true;
	}

	protected bool IsRoomIsolated(Model.Room room)
	{
		throw new NotImplementedException();
	}

	/// <summary>
	/// Checks if a given room is isolated, meaning it is surrounded by inactive grid cells in all cardinal directions.
	/// </summary>
	/// <param name="room">The room to check.</param>
	/// <returns>Returns true if the room is isolated, otherwise false.</returns>
	protected bool IsRoomIsolated(RectangleRoom room)
	{
		if (!IsRoomAvailable(room))
		{
			return false;
		}

		Vector2I placeholder = new Vector2I(Grid.Current.Position.X, Grid.Current.Position.Y);

		HashSet<GeneratorGrid.Direction> eastWest = new HashSet<GeneratorGrid.Direction>();
		HashSet<GeneratorGrid.Direction> northSouth = new HashSet<GeneratorGrid.Direction>();
		GridCell result;

		// Check values along x axis at y intersections.
		for (int x = room.TopLeft.X; x < room.TopLeft.X + room.Size.X; x++)
		{
			Grid.MoveTo(new Vector2I(x, room.TopLeft.Y));
			result = Grid.RelativeQuery(GeneratorGrid.Direction.North);
			if (result != null && result.IsActive)
			{
				Grid.MoveTo(placeholder);
				return false;
			}
			
			Grid.MoveTo(new Vector2I(x, room.TopLeft.Y + room.Size.Y));
			result = Grid.RelativeQuery(GeneratorGrid.Direction.South);
			if (result != null && result.IsActive)
			{
				Grid.MoveTo(placeholder);
				return false;
			}
		}

		// Now check values along y axis at x intersections.
		for (int y = room.TopLeft.Y; y < (room.TopLeft.Y + room.Size.Y); y++)
		{
			Grid.MoveTo(new Vector2I(room.TopLeft.X, y));
			result = Grid.RelativeQuery(GeneratorGrid.Direction.West);
			if (result != null && result.IsActive)
			{
				Grid.MoveTo(placeholder);
				return false;
			}
		    
		    Grid.MoveTo(new Vector2I(room.TopLeft.X + room.Size.X, y));
		    result = Grid.RelativeQuery(GeneratorGrid.Direction.East);
		    if ( result != null && result.IsActive )
			{
				Grid.MoveTo(placeholder);
				return false;
			}
		}

		return true;
	}
}