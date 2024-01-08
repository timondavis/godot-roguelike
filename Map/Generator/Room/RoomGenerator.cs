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

	protected async void Generate()
	{
		NumberOfRooms = GD.RandRange(RoomCountMin, RoomCountMax);
		await PlaceRooms();
		await ConnectRooms();
		EmitSignal(SignalName.MapFinalized);	
	}

	protected abstract Task PlaceRooms();

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

	protected virtual bool IsRoomAvailable(int startX, int startY, int roomWidth, int roomHeight)
	{
		for (int x = startX; x < startX + roomWidth; x++)
		{
			for (int y = startY; y < startY + roomHeight; y++)
			{
				Grid.MoveTo(new Vector2I(x,y));
				if (Grid.Current.IsActive)
				{
					return false;
				};
			}
		}

		return true;
	}
}