using Godot;
using System;
using System.Collections.Generic;
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
		NumberOfRooms = GD.RandRange(RoomCountMin, RoomCountMax);
		_PlaceRooms();
	}

	private async void _PlaceRooms()
	{

		int currentRoomCounter = 0;
		int attemptsForCurrentRoom = 0;
		bool isRoomPlaced;
		while (currentRoomCounter < NumberOfRooms && attemptsForCurrentRoom < RoomApplicationAttemptsMax )
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

		EmitSignal(SignalName.MapFinalized, Grid);	
	}

	private bool _PlaceRoom()
	{
		int roomWidth = GD.RandRange(RoomSizeMin, RoomSizeMax);
		int roomHeight = GD.RandRange(RoomSizeMin, RoomSizeMax);
		int startX = GD.RandRange(0, Width - roomWidth);
		int startY = GD.RandRange(0, Height - roomHeight);
		TileType floorTileType = TileTypes.FindByName(TileType_Floor);
		
		// Return false if conflict is found.
		if (_IsRoomAvailable(startX, startY, roomWidth, roomHeight))
		{
			for (int x = startX; x < startX + roomWidth; x++)
			{
				for (int y = startY; y < startY + roomHeight; y++)
				{
					Grid.MoveTo(new Vector2I(x,y));
					Grid.Current.Activate(floorTileType);
					Rooms.Add(new RectangleRoom
					{
						Center = new Vector2I(startX, startY),
						Size = new Vector2I(roomWidth, roomHeight)
					});
				}
			}
			
			return true;
		}
		
		return false;
	}

	private bool _IsRoomAvailable(int startX, int startY, int roomWidth, int roomHeight)
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
