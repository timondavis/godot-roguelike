using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Roguelike.Map.Generator;
using Roguelike.Map.Model;
using static Godot.PropertyHint;

public partial class BasicRoomPlacementGenerator : MapGenerator
{
	public const string TileType_Floor = "floor";
	
	[Export(PropertyHint.Range, "3,1000,1")] 
	public int RoomSizeMin { get; set; }
	[Export(PropertyHint.Range, "3,1000,1")] 
	public int RoomSizeMax { get; set; }
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMin { get; set; }
	[Export(PropertyHint.Range, "1,1000,1")] public int RoomCountMax { get; set; }
	
	[Export] public float CycleEmissionDelay { get; set; }

	public int NumberOfRooms = 0;
	public int RoomApplicationAttemptsMax = 100;
	public HashSet<RectangleRoom> Rooms = new HashSet<RectangleRoom>();
	
	public BasicRoomPlacementGenerator() : base()
	{
		TileTypes.Add(new TileType { Name=TileType_Floor } );
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		RoomSizeMin = Math.Max(3, RoomSizeMin);
		RoomSizeMax = Math.Max(3, RoomSizeMax);
		RoomCountMin = Math.Max(1, RoomCountMin);
		RoomCountMax = Math.Max(1, RoomCountMax);
		GenerateGrid();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void GenerateGrid()
	{
		if (RoomSizeMax < RoomSizeMin)
		{
			throw new ArgumentException("RoomSizeMax cannot be less than RoomSizeMin");
		}

		if (RoomCountMax < RoomCountMin)
		{
			throw new ArgumentException("RoomCountMax cannot be less than RoomCountMin");
		}
		
		InitializeGrid();
		GD.Randomize();
		_Generate();
	}

	protected async void _Generate()
	{
		NumberOfRooms = GD.RandRange(RoomCountMin, RoomCountMax);
		await _PlaceRooms();
		await _ConnectRooms();
		EmitSignal(SignalName.MapFinalized);	
	}

	protected virtual async Task _PlaceRooms()
	{
		int currentRoomCounter = 0;
		int attemptsForCurrentRoom = 0;
		bool isRoomPlaced;
		while (currentRoomCounter < NumberOfRooms && attemptsForCurrentRoom < RoomApplicationAttemptsMax)
		{
			isRoomPlaced = _PlaceRoom();
			if (isRoomPlaced)
			{
				currentRoomCounter++;
				attemptsForCurrentRoom = 0;

				if (CycleEmissionDelay > 0)
				{
					await ToSignal(GetTree().CreateTimer(CycleEmissionDelay), SceneTreeTimer.SignalName.Timeout);
					EmitSignal(SignalName.MapUpdated, Grid);
				}
			}
			else
			{
				attemptsForCurrentRoom++;
			}
		}
	}

	protected virtual async Task _ConnectRooms()
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

	protected virtual bool _PlaceRoom()
	{
		int roomWidth = GD.RandRange(RoomSizeMin, RoomSizeMax);
		int roomHeight = GD.RandRange(RoomSizeMin, RoomSizeMax);
		int startX = GD.RandRange(0, Width - roomWidth);
		int startY = GD.RandRange(0, Height - roomHeight);
		TileType floorTileType = TileTypes.FindByName(TileType_Floor);
		
		// Return false if conflict is found.
		if (_IsRoomAvailable(startX, startY, roomWidth, roomHeight))
		{
			double centerX = (double)(startX) + (double)(roomWidth / 2.0);
			double centerY = (double)(startY) + (double)(roomHeight / 2.0);
			RectangleRoom room  = new RectangleRoom
			{
				Center = new Vector2I( (int) Math.Floor( centerX ), (int) Math.Floor( centerY ) ),
				TopLeft = new Vector2I(startX, startY),
				Size = new Vector2I(roomWidth, roomHeight)
			};
			
			Grid.FillRect(room.TopLeft,room.Size, floorTileType);
			Rooms.Add(room);
			
			return true;
		}
		
		return false;
	}

	protected virtual bool _IsRoomAvailable(int startX, int startY, int roomWidth, int roomHeight)
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
